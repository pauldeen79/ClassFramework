namespace ClassFramework.Pipelines.Builders;

public class PropertyBuilderWrapper : IBuilder<Property>
{
    public PropertyBuilder Builder { get; } = new();

    public Property Build() => Builder.Build();
}
