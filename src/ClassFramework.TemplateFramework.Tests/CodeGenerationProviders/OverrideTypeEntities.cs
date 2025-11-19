namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class OverrideTypeEntities(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    public override string Path => "Test.Domain/Types";

    protected override bool EnableEntityInheritance => true;
    protected override Task<Result<TypeBase>> GetBaseClassAsync() => CreateBaseClassAsync(typeof(IAbstractBase), "Test.Domain");

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token)
        => GetEntitiesAsync(GetOverrideModelsAsync(typeof(IAbstractBase)), "Test.Domain.Types");
}
