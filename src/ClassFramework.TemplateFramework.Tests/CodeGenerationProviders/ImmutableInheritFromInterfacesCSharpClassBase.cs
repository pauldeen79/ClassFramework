namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableInheritFromInterfacesCSharpClassBase(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    protected override bool InheritFromInterfaces => true;
}
