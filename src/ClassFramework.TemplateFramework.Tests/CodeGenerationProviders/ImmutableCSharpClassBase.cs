﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableCSharpClassBase(IPipelineService pipelineService) : CsharpClassGeneratorPipelineCodeGenerationProviderBase(pipelineService)
{
    public override bool RecurseOnDeleteGeneratedFiles => false;
    public override string LastGeneratedFilesFilename => string.Empty;
    public override Encoding Encoding => Encoding.UTF8;

    protected override Type EntityCollectionType => typeof(IReadOnlyCollection<>);
    protected override Type EntityConcreteCollectionType => typeof(List<>);
    protected override Type BuilderCollectionType => typeof(List<>);

    protected override string ProjectName => "Test";
    protected override string CodeGenerationRootNamespace => "ClassFramework.TemplateFramework.Tests";
    protected override string CoreNamespace => "Test.Domain";
    protected override bool CopyAttributes => true;
    protected override bool CopyInterfaces => true;
    protected override bool CreateCodeGenerationHeader => false;
    protected override bool AddImplicitOperatorOnBuilder => false;
    protected override bool UseBuilderAbstractionsTypeConversion => false;

    protected async Task<Result<IEnumerable<TypeBase>>> GetTemplateFrameworkModelsAsync()
        => await GetNonCoreModelsAsync($"{CodeGenerationRootNamespace}.Models.TemplateFramework").ConfigureAwait(false);

    protected async Task<Result<IEnumerable<TypeBase>>> GetInheritFromInterfacesModelsAsync()
        => await GetNonCoreModelsAsync($"{CodeGenerationRootNamespace}.Models.InheritFromInterfaces").ConfigureAwait(false);

    protected override string[] GetAdditionalSkippedNamespacesOnTypenameMappings() =>
    [
        $"{CodeGenerationRootNamespace}.Models.TemplateFramework",
    ];
}
