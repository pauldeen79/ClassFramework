namespace ClassFramework.Pipelines.Interface.Components;

public class AddMethodsComponent : IPipelineComponent<InterfaceContext, InterfaceBuilder>
{
    public Task<Result> ExecuteAsync(InterfaceContext context, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            if (!context.Settings.CopyMethods)
            {
                return Result.Continue();
            }

            response.AddMethods(context.SourceModel.Methods
                .Where(x => context.Settings.CopyMethodPredicate is null || context.Settings.CopyMethodPredicate(context.SourceModel, x))
                .Select(x => x.ToBuilder()
                    .WithReturnTypeName(context.MapTypeName(x.ReturnTypeName.FixCollectionTypeName(context.Settings.EntityNewCollectionTypeName).FixNullableTypeName(new TypeContainerWrapper(x)), MetadataNames.CustomEntityInterfaceTypeName))
                    .With(y => y.Parameters.ToList().ForEach(z => z.TypeName = context.MapTypeName(z.TypeName, MetadataNames.CustomEntityInterfaceTypeName)))
                    .With(y => y.WithNew(context.Settings.UseBuilderAbstractionsTypeConversion && response.Interfaces.Any() && !response.Interfaces.Contains(y.ReturnTypeName)))
                ));

            return Result.Success();
        }, token);
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

    public ITypeContainerBuilder ToBuilder() => new TypeContainerWrapperBuilder(this);
}

[ExcludeFromCodeCoverage]
internal sealed class TypeContainerWrapperBuilder : ITypeContainerBuilder
{
    public TypeContainerWrapperBuilder(TypeContainerWrapper typeContainerWrapper)
    {
        TypeName = typeContainerWrapper.TypeName;
        IsNullable = typeContainerWrapper.IsNullable;
        IsValueType = typeContainerWrapper.IsValueType;
        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(typeContainerWrapper.GenericTypeArguments.Select(x => x.ToBuilder()).ToList());
    }

    public TypeContainerWrapperBuilder()
    {
        TypeName = string.Empty;
        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>();
    }

    public string TypeName { get; set; }
    public bool IsNullable { get; set; }
    public bool IsValueType { get; set; }
    public ObservableCollection<ITypeContainerBuilder> GenericTypeArguments { get; set; }

    public ITypeContainer Build()
        => new TypeContainerWrapper(new MethodBuilder()
            .WithName("Dummy")
            .WithReturnTypeName(TypeName)
            .WithReturnTypeIsNullable(IsNullable)
            .WithReturnTypeIsValueType(IsValueType));
}
