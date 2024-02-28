namespace ClassFramework.Domain.Builders;

public partial class AttributeBuilder
{
    public AttributeBuilder AddNameAndParameter(string name, object value)
        => this.WithName(name).AddParameters(new AttributeParameterBuilder().WithValue(value));

    public AttributeBuilder ForCodeGenerator(string generatorName, string generatorVersion)
        => this.WithName(typeof(GeneratedCodeAttribute).FullName)
            .AddParameters
            (
                new AttributeParameterBuilder().WithValue(generatorName),
                new AttributeParameterBuilder().WithValue(generatorVersion)
            );

    public AttributeBuilder WithName(Type sourceType)
        => this.WithName(sourceType.IsNotNull(nameof(sourceType)).FullName.FixTypeName());
}
