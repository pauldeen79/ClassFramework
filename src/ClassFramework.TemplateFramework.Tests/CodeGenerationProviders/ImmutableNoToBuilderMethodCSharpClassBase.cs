namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableNoToBuilderMethodCSharpClassBase : ImmutableCSharpClassBase
{
    protected ImmutableNoToBuilderMethodCSharpClassBase(IPipelineService pipelineService) : base(pipelineService)
    {
    }

    protected override SubVisibility SetterVisibility => SubVisibility.Private;
    protected override string ToBuilderFormatString => string.Empty;
    protected override string ToTypedBuilderFormatString => string.Empty;
}
