namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableInheritFromInterfacesCSharpClassBase : ImmutableCSharpClassBase
{
    protected ImmutableInheritFromInterfacesCSharpClassBase(IPipelineService pipelineService, ICsharpExpressionDumper csharpExpressionDumper) : base(pipelineService, csharpExpressionDumper)
    {
    }

    protected override bool InheritFromInterfaces => true;
}
