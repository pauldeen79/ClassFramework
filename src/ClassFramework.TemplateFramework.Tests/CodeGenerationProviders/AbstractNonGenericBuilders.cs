namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class AbstractNonGenericBuilders(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken cancellationToken) => GetNonGenericBuildersAsync(GetAbstractModelsAsync(), "Test.Domain.Builders", "Test.Domain");

    public override string Path => "Test.Domain/Builders";

    protected override bool AddNullChecks => false; // not needed for abstract builders, because each derived class will do its own validation
    protected override bool AddBackingFields => true; // backing fields are added when using null checks... so we need to add this explicitly

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override bool IsAbstract => true;
    protected override bool CreateAsObservable => true;

    protected override string FilenameSuffix => ".nongeneric.template.generated";

    // Do not generate 'With' methods. Do this on the interfaces instead.
    protected override string SetMethodNameFormatString => string.Empty;
    protected override string AddMethodNameFormatString => string.Empty;
}
