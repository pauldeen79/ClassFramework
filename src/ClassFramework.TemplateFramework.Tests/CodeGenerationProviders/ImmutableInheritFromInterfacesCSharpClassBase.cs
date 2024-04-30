namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableInheritFromInterfacesCSharpClassBase : ImmutableCSharpClassBase
{
    protected ImmutableInheritFromInterfacesCSharpClassBase(IMediator mediator) : base(mediator)
    {
    }

    protected override bool InheritFromInterfaces => true;
}
