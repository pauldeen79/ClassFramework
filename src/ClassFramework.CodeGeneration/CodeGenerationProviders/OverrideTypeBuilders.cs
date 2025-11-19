namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class OverrideTypeBuilders(ICommandService commandService) : ClassFrameworkCSharpClassBase(commandService)
{
    public override string Path => "ClassFramework.Domain/Builders/Types";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override Task<Result<TypeBase>> GetBaseClassAsync() => CreateBaseClassAsync(typeof(ITypeBase), "ClassFramework.Domain");

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token)
        => GetBuildersAsync(GetOverrideModelsAsync(typeof(ITypeBase)), "ClassFramework.Domain.Builders.Types", "ClassFramework.Domain.Types");
}
