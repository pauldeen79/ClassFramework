namespace ClassFramework.IntegrationTests.CodeGenerationProviders;

public abstract class TestCodeGenerationProviderBase : CsharpClassGeneratorPipelineCodeGenerationProviderBase
{
    protected TestCodeGenerationProviderBase(ICsharpExpressionCreator csharpExpressionCreator, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<IConcreteTypeBuilder, OverrideEntityContext> overrideEntityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionCreator, builderPipeline, builderExtensionPipeline, entityPipeline, overrideEntityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override bool RecurseOnDeleteGeneratedFiles => false;
    public override string LastGeneratedFilesFilename => string.Empty;
    public override Encoding Encoding => Encoding.UTF8;

    protected override Type RecordCollectionType => typeof(IReadOnlyCollection<>);
    protected override Type RecordConcreteCollectionType => typeof(List<>);
    protected override Type BuilderCollectionType => typeof(List<>);
    //protected override Type RecordCollectionType => typeof(ObservableCollection<>);
    //protected override Type RecordConcreteCollectionType => typeof(ObservableCollection<>);
    //protected override Type BuilderCollectionType => typeof(ObservableCollection<>);

    protected override string ProjectName => "ClassFramework";
    protected override string CodeGenerationRootNamespace => "ClassFramework.IntegrationTests";
    protected override string CoreNamespace => "ClassFramework.Domain";
    protected override bool CopyAttributes => true;
    protected override bool CopyInterfaces => true;
    //protected override ArgumentValidationType ValidateArgumentsInConstructor => ArgumentValidationType.Shared;
    //protected override Domain.Domains.SubVisibility SetterVisibility => Domain.Domains.SubVisibility.Private;
    //protected override bool AddBackingFields => true;
    //protected override bool CreateAsObservable => true;
    //protected override string ToBuilderFormatString => string.Empty;
    //protected override string ToTypedBuilderFormatString => string.Empty;
    //protected override bool AddFullConstructor => false;
    //protected override bool AddPublicParameterlessConstructor => true;
    //protected override bool AddCopyConstructor => false;

    protected TypeBase[] GetPipelineModels()
        => GetNonCoreModels($"{CodeGenerationRootNamespace}.Models.Pipelines");

    protected TypeBase[] GetTemplateFrameworkModels()
        => GetNonCoreModels($"{CodeGenerationRootNamespace}.Models.TemplateFramework");

    protected override bool SkipNamespaceOnTypenameMappings(string @namespace)
        => base.SkipNamespaceOnTypenameMappings(@namespace)
        && !@namespace.In($"{CodeGenerationRootNamespace}.Models.Pipelines",
                           $"{CodeGenerationRootNamespace}.Models.TemplateFramework");
}
