namespace ClassFramework.Pipelines.Entity.Components;

public class AddPropertiesComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext> Build()
        => new AddPropertiesComponent();
}

public class AddPropertiesComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> Process(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var properties = context.Request.SourceModel
            .Properties
            .Where(property => context.Request.SourceModel.IsMemberValidForBuilderClass(property, context.Request.Settings))
            .ToArray();

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

        return Task.FromResult(Result.Continue());
    }

    private static void AddBackingFields(PipelineContext<EntityContext> context, Property[] properties)
        => context.Request.Builder.AddFields
        (
            properties
                .Select
                (
                    property => new FieldBuilder()
                        .WithName($"_{property.Name.ToPascalCase(context.Request.FormatProvider.ToCultureInfo())}")
                        .WithTypeName(context.Request.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName)
                            .FixCollectionTypeName(context.Request.Settings.CollectionTypeName
                                .WhenNullOrEmpty(context.Request.Settings.EntityNewCollectionTypeName)
                                .WhenNullOrEmpty(typeof(List<>).WithoutGenerics()))
                        .FixNullableTypeName(property))
                        .WithIsNullable(property.IsNullable)
                        .WithIsValueType(property.IsValueType)
                        .AddGenericTypeArguments(property.GenericTypeArguments)
                )
        );

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertyGetterStatements(
        Property property,
        EntityContext context)
    {
        if (context.Settings.AddBackingFields || context.Settings.CreateAsObservable)
        {
            yield return new StringCodeStatementBuilder().WithStatement($"return _{property.Name.ToPascalCase(context.FormatProvider.ToCultureInfo())};");
        }
    }

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertySetterStatements(
        Property property,
        EntityContext context)
    {
        if (context.Settings.AddBackingFields || context.Settings.CreateAsObservable)
        {
            yield return new StringCodeStatementBuilder().WithStatement($"_{property.Name.ToPascalCase(context.FormatProvider.ToCultureInfo())} = value{property.GetNullCheckSuffix("value", context.Settings.AddNullChecks, context.SourceModel)};");
            if (context.Settings.CreateAsObservable)
            {
                yield return new StringCodeStatementBuilder().WithStatement($"HandlePropertyChanged(nameof({property.Name}));");
            }
        }
    }
}
