namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableUseBuilderAbstractionsTypeConversionCSharpClassBase(ICommandService commandService) : ImmutableCSharpClassBase(commandService)
{
    protected override bool UseBuilderAbstractionsTypeConversion => true;
    protected override string[] GetBuilderAbstractionsTypeConversionNamespaces() => [ "Test.Abstractions" ];
}
