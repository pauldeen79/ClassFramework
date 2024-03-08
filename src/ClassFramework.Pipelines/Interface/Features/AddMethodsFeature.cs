namespace ClassFramework.Pipelines.Interface.Features;

public class AddMethodsFeatureBuilder : IInterfaceFeatureBuilder
{
    public IPipelineFeature<InterfaceBuilder, InterfaceContext> Build()
        => new AddMethodsFeature();
}

public class AddMethodsFeature : IPipelineFeature<InterfaceBuilder, InterfaceContext>
{
    public Result<InterfaceBuilder> Process(PipelineContext<InterfaceBuilder, InterfaceContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CopyMethods)
        {
            return Result.Continue<InterfaceBuilder>();
        }

        context.Model.AddMethods(context.Context.SourceModel.Methods
            .Where(x => context.Context.Settings.CopyMethodPredicate is null || context.Context.Settings.CopyMethodPredicate(context.Context.SourceModel, x))
            .Select(x => x.ToBuilder()
                .WithReturnTypeName(context.Context.MapTypeName(x.ReturnTypeName.FixCollectionTypeName(context.Context.Settings.EntityNewCollectionTypeName).FixNullableTypeName(new TypeContainerWrapper(x)), MetadataNames.CustomEntityInterfaceTypeName))
                .With(y => y.Parameters.ForEach(z => z.TypeName = context.Context.MapTypeName(z.TypeName, MetadataNames.CustomEntityInterfaceTypeName)))
            ));

        return Result.Continue<InterfaceBuilder>();
    }

    public IBuilder<IPipelineFeature<InterfaceBuilder, InterfaceContext>> ToBuilder()
        => new AddMethodsFeatureBuilder();
}

[ExcludeFromCodeCoverage]
internal sealed class TypeContainerWrapper : ITypeContainer
{
    public TypeContainerWrapper(Method method)
    {
        method = method.IsNotNull(nameof(method));
        TypeName = method.ReturnTypeName;
        IsNullable = method.ReturnTypeIsNullable;
        IsValueType = method.ReturnTypeIsValueType;
    }

    public string TypeName { get; }
    public bool IsNullable { get; }
    public bool IsValueType { get; }
}
