namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableNoToBuilderMethodCSharpClassBase(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    protected override SubVisibility SetterVisibility => SubVisibility.Private;
    protected override string ToBuilderFormatString => string.Empty;
    protected override string ToTypedBuilderFormatString => string.Empty;
}
