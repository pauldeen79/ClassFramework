namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutablePrivateSettersCSharpClassBase : ImmutableCSharpClassBase
{
    protected ImmutablePrivateSettersCSharpClassBase(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    protected override SubVisibility SetterVisibility => SubVisibility.Private;
    protected override bool AddSetters => true;
}
