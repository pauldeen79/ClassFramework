﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableCSharpClassBase : CsharpClassGeneratorPipelineCodeGenerationProviderBase
{
    protected ImmutableCSharpClassBase(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

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

    protected TypeBase[] GetTemplateFrameworkModels()
        => GetNonCoreModels($"{CodeGenerationRootNamespace}.Models.TemplateFramework");

    protected override bool SkipNamespaceOnTypenameMappings(string @namespace)
        => @namespace == $"{CodeGenerationRootNamespace}.Models.TemplateFramework";

}
