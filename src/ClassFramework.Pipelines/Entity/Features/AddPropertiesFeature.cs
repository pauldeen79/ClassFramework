namespace ClassFramework.Pipelines.Entity.Features;

public class AddPropertiesFeatureBuilder : IEntityFeatureBuilder
{
    public IPipelineFeature<IConcreteTypeBuilder, EntityContext> Build()
        => new AddPropertiesFeature();
}

public class AddPropertiesFeature : IPipelineFeature<IConcreteTypeBuilder, EntityContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        var properties = context.Context.SourceModel
            .Properties
            .Where(property => context.Context.SourceModel.IsMemberValidForBuilderClass(property, context.Context.Settings))
            .ToArray();

        context.Model.AddProperties(
            properties.Select
            (
                property => context.Context.CreatePropertyForEntity(property)
                    .WithVirtual(property.Virtual)
                    .WithAbstract(property.Abstract)
                    .WithProtected(property.Protected)
                    .WithOverride(property.Override)
                    .WithHasInitializer(property.HasInitializer && !(context.Context.Settings.AddSetters || context.Context.Settings.AddBackingFields || context.Context.Settings.CreateAsObservable))
                    .WithHasSetter(context.Context.Settings.AddSetters || context.Context.Settings.AddBackingFields || context.Context.Settings.CreateAsObservable)
                    .WithGetterVisibility(property.GetterVisibility)
                    .WithSetterVisibility(context.Context.Settings.SetterVisibility)
                    .WithInitializerVisibility(property.InitializerVisibility)
                    .WithExplicitInterfaceName(property.ExplicitInterfaceName)
                    .WithParentTypeFullName(property.ParentTypeFullName)
                    .AddGetterCodeStatements(CreateBuilderPropertyGetterStatements(property, context.Context))
                    .AddSetterCodeStatements(CreateBuilderPropertySetterStatements(property, context.Context))
            )
        );

        if (context.Context.Settings.AddBackingFields || context.Context.Settings.CreateAsObservable)
        {
            AddBackingFields(context, properties);
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }

    private static void AddBackingFields(PipelineContext<IConcreteTypeBuilder, EntityContext> context, Property[] properties)
        => context.Model.AddFields
        (
            properties
                .Select
                (
                    property =>new FieldBuilder()
                        .WithName($"_{property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo())}")
                        .WithTypeName(context.Context.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName)
                            .FixCollectionTypeName(context.Context.Settings.CollectionTypeName
                                .WhenNullOrEmpty(context.Context.Settings.EntityNewCollectionTypeName)
                                .WhenNullOrEmpty(typeof(List<>).WithoutGenerics()))
                        .FixNullableTypeName(property))
                        .WithIsNullable(property.IsNullable)
                        .WithIsValueType(property.IsValueType)
                )
        );

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, EntityContext>> ToBuilder()
        => new AddPropertiesFeatureBuilder();

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
            yield return new StringCodeStatementBuilder().WithStatement($"_{property.Name.ToPascalCase(context.FormatProvider.ToCultureInfo())} = value{property.GetNullCheckSuffix("value", context.Settings.AddNullChecks)};");
            if (context.Settings.CreateAsObservable)
            {
                yield return new StringCodeStatementBuilder().WithStatement($"HandlePropertyChanged(nameof({property.Name}));");
            }
        }
    }
}
