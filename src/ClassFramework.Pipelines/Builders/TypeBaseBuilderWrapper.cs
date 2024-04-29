namespace ClassFramework.Pipelines.Builders;

public class TypeBaseBuilderWrapper : IBuilder<TypeBase>
{
    public TypeBaseBuilderWrapper(Type sourceModel)
    {
        sourceModel = sourceModel.IsNotNull(nameof(sourceModel));

        Builder = sourceModel.IsInterface
            ? new InterfaceBuilder()
            : new ClassBuilder();
    }

    public TypeBaseBuilder Builder { get; }

    public TypeBase Build() => Builder.Build();
}
