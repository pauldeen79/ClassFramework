namespace ClassFramework.Domain.Builders;

public partial class ConstructorBuilder
{
    public ConstructorBuilder ChainCallToBaseUsingParameters()
        => WithChainCall($"base({string.Join(", ", Parameters.Select(x => x.Name))})");
}
