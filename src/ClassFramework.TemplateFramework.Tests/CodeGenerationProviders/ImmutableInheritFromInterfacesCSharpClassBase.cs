namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableInheritFromInterfacesCSharpClassBase(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    protected override string BuilderAbstractionsNamespace => $"{ProjectName}.Abstractions.Builders";
    protected override string AbstractionsNamespace => $"{ProjectName}.Abstractions";
    protected override string DomainsNamespace => $"{ProjectName}.Abstractions.Domains";
    protected override string ValidationNamespace => $"{ProjectName}.Abstractions.Validation";
    protected override bool UseBuilderAbstractionsTypeConversion => true;
}
