namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutablePrivateSettersCSharpClassBase(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    protected override SubVisibility SetterVisibility => SubVisibility.Private;
    protected override bool AddSetters => true;
}
