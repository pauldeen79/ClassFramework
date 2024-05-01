namespace ClassFramework.Pipelines.Builders;

public class InterfaceBuilderWrapper : IBuilder<Domain.Types.Interface>
{
    public InterfaceBuilder Builder { get; } = new();

    public Domain.Types.Interface Build() => Builder.BuildTyped();
}
