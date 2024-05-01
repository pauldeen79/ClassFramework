namespace ClassFramework.Pipelines.Builders;

public class ClassBuilderWrapper : IBuilder<Class>, IBuilder<IConcreteType>
{
    public ClassBuilder Builder { get; } = new();

    public Class Build() => Builder.BuildTyped();

    IConcreteType IBuilder<IConcreteType>.Build() => Build();
}
