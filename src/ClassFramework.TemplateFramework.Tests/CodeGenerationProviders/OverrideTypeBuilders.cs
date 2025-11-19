namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class OverrideTypeBuilders(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    public override string Path => "Test.Domain/Builders/Types";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override Task<Result<TypeBase>> GetBaseClassAsync() => CreateBaseClassAsync(typeof(IAbstractBase), "Test.Domain");

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token)
        => GetBuildersAsync(GetOverrideModelsAsync(typeof(IAbstractBase)), "Test.Domain.Builders.Types", "Test.Domain.Types");
}
