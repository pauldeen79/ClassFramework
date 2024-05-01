namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableInheritFromInterfacesCSharpClassBase : ImmutableCSharpClassBase
{
    protected ImmutableInheritFromInterfacesCSharpClassBase(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
    {
    }

    protected override bool InheritFromInterfaces => true;
}
