﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableUseBuilderAbstractionsTypeConversionCSharpClassBase(IPipelineService pipelineService) : ImmutableCSharpClassBase(pipelineService)
{
    protected override bool UseBuilderAbstractionsTypeConversion => true;
    protected override string[] GetBuilderAbstractionsTypeConversionNamespaces() => [ "Test.Abstractions" ];
}
