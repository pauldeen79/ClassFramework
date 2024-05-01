namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsBuildersInterfaces : ClassFrameworkCSharpClassBase
{
    public AbstractionsBuildersInterfaces(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilderInterfaces(await GetAbstractionsInterfaces().ConfigureAwait(false), "ClassFramework.Domain.Builders.Abstractions", "ClassFramework.Domain.Abstractions", "ClassFramework.Domain.Builders.Abstractions").ConfigureAwait(false);

    public override string Path => "ClassFramework.Domain/Builders/Abstractions";

    protected override bool EnableEntityInheritance => true;
}
