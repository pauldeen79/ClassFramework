namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutablePrivateSettersCSharpClassBase : ImmutableCSharpClassBase
{
    protected ImmutablePrivateSettersCSharpClassBase(IMediator mediator) : base(mediator)
    {
    }

    protected override SubVisibility SetterVisibility => SubVisibility.Private;
    protected override bool AddSetters => true;
}
