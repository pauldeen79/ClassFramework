namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class MappedCSharpClassBase(ICommandService commandService) : CsharpClassGeneratorPipelineCodeGenerationProviderBase(commandService)
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

    protected override IEnumerable<TypenameMappingBuilder> GetAdditionalTypenameMappings()
    {
        yield return new TypenameMappingBuilder()
            .WithSourceType(typeof(IMyMappedType))
            .WithTargetType(typeof(IMyMappedType))
            .AddMetadata(CreateTypenameMappingMetadata(typeof(IMyMappedType)));
    }

    protected Task<Result<IEnumerable<TypeBase>>> GetTypeNamedModelsAsync()
        => GetNonCoreModelsAsync($"{CodeGenerationRootNamespace}.SomeNamespace");
}
