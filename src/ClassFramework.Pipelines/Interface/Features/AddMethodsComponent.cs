namespace ClassFramework.Pipelines.Interface.Features;

public class AddMethodsComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceBuilder, InterfaceContext> Build()
        => new AddMethodsComponent();
}

public class AddMethodsComponent : IPipelineComponent<InterfaceBuilder, InterfaceContext>
{
    public Task<Result<InterfaceBuilder>> Process(PipelineContext<InterfaceBuilder, InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CopyMethods)
        {
            return Task.FromResult(Result.Continue<InterfaceBuilder>());
        }

        context.Model.AddMethods(context.Context.SourceModel.Methods
            .Where(x => context.Context.Settings.CopyMethodPredicate is null || context.Context.Settings.CopyMethodPredicate(context.Context.SourceModel, x))
            .Select(x => x.ToBuilder()
                .WithReturnTypeName(context.Context.MapTypeName(x.ReturnTypeName.FixCollectionTypeName(context.Context.Settings.EntityNewCollectionTypeName).FixNullableTypeName(new TypeContainerWrapper(x)), MetadataNames.CustomEntityInterfaceTypeName))
                .With(y => y.Parameters.ToList().ForEach(z => z.TypeName = context.Context.MapTypeName(z.TypeName, MetadataNames.CustomEntityInterfaceTypeName)))
            ));

        return Task.FromResult(Result.Continue<InterfaceBuilder>());
    }
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
        GenericTypeArguments = method.ReturnTypeGenericTypeArguments;
    }

    public string TypeName { get; }
    public bool IsNullable { get; }
    public bool IsValueType { get; }

    public IReadOnlyCollection<ITypeContainer> GenericTypeArguments { get; }
}
