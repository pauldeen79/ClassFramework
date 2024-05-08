namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableInheritFromInterfacesCSharpClassBase : ImmutableCSharpClassBase
{
    protected ImmutableInheritFromInterfacesCSharpClassBase(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    protected override bool InheritFromInterfaces => true;
}
