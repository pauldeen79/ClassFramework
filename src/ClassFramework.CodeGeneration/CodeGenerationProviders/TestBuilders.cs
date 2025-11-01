namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class TestBuilders(ICommandService commandService) : ClassFrameworkCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetBuildersAsync(GetTestModelsAsync(), "ClassFramework.TemplateFramework.Tests.Builders", "ClassFramework.TemplateFramework.Tests");

    public override string Path => "ClassFramework.TemplateFramework.Tests/Builders";

    protected override bool CreateAsObservable => true;
    protected override bool UseBuilderLazyValues => true;
}
