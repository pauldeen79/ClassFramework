namespace ClassFramework.Pipelines.Entity.Components;

public class AddPropertiesComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            var properties = context.Request.GetSourceProperties().ToArray();

            context.Request.Builder.AddProperties(
                properties.Select
                (
                    property => context.Request.CreatePropertyForEntity(property)
                        .WithVirtual(property.Virtual)
                        .WithAbstract(property.Abstract)
                        .WithProtected(property.Protected)
                        .WithOverride(property.Override)
                        .WithHasInitializer(property.HasInitializer && !(context.Request.Settings.AddSetters || context.Request.Settings.AddBackingFields || context.Request.Settings.CreateAsObservable))
                        .WithHasSetter(((context.Request.Settings.AddSetters || context.Request.Settings.AddBackingFields) && !property.TypeName.IsCollectionTypeName()) || context.Request.Settings.CreateAsObservable)
                        .WithGetterVisibility(property.GetterVisibility)
                        .WithSetterVisibility(context.Request.Settings.SetterVisibility)
                        .WithInitializerVisibility(property.InitializerVisibility)
                        .WithExplicitInterfaceName(property.ExplicitInterfaceName)
                        .WithParentTypeFullName(property.ParentTypeFullName)
                        .AddGetterCodeStatements(CreateBuilderPropertyGetterStatements(property, context.Request))
                        .AddSetterCodeStatements(CreateBuilderPropertySetterStatements(property, context.Request))
                )
            );

            if (context.Request.Settings.AddBackingFields || context.Request.Settings.CreateAsObservable)
            {
                AddBackingFields(context, properties);
            }

            return Result.Success();
        }, token);

    private static void AddBackingFields(PipelineContext<EntityContext> context, Property[] properties)
        => context.Request.Builder.AddFields
        (
            properties
                .Select
                (
                    property => new FieldBuilder()
                        .WithName($"_{property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo())}")
                        .WithTypeName(context.Request.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName)
                            .FixCollectionTypeName(context.Request.Settings.CollectionTypeName
                                .WhenNullOrEmpty(context.Request.Settings.EntityNewCollectionTypeName)
                                .WhenNullOrEmpty(typeof(List<>).WithoutGenerics()))
                        .FixNullableTypeName(property))
                        .WithIsNullable(property.IsNullable)
                        .WithIsValueType(property.IsValueType)
                        .AddGenericTypeArguments(property.GenericTypeArguments.Select(x => x.ToBuilder()))
                )
        );

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertyGetterStatements(Property property, EntityContext context)
    {
        if (context.Settings.AddBackingFields || context.Settings.CreateAsObservable)
        {
            yield return new StringCodeStatementBuilder($"return _{property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo())};");
        }
    }

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertySetterStatements(Property property, EntityContext context)
    {
        if (context.Settings.AddBackingFields || context.Settings.CreateAsObservable)
        {
            if (context.Settings.CreateAsObservable)
            {
                var nullSuffix = context.Settings.EnableNullableReferenceTypes && !property.IsValueType
                    ? "!"
                    : string.Empty;
                yield return new StringCodeStatementBuilder($"bool hasChanged = !{typeof(EqualityComparer<>).WithoutGenerics()}<{context.CreatePropertyForEntity(property).TypeName}>.Default.Equals(_{property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo())}{nullSuffix}, value{nullSuffix});");
            }

            yield return new StringCodeStatementBuilder($"_{property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo())} = value{property.GetNullCheckSuffix("value", context.Settings.AddNullChecks, context.SourceModel)};");

            if (context.Settings.CreateAsObservable)
            {
                yield return new StringCodeStatementBuilder($"if (hasChanged) HandlePropertyChanged(nameof({property.Name}));");
            }
        }
    }
}
