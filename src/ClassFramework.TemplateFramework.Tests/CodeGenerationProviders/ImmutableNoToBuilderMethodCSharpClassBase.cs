﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class ImmutableNoToBuilderMethodCSharpClassBase : ImmutableCSharpClassBase
{
    protected ImmutableNoToBuilderMethodCSharpClassBase(ICsharpExpressionCreator csharpExpressionCreator, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<IConcreteTypeBuilder, OverrideEntityContext> overrideEntityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionCreator, builderPipeline, builderExtensionPipeline, entityPipeline, overrideEntityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    protected override SubVisibility SetterVisibility => SubVisibility.Private;
    protected override string ToBuilderFormatString => string.Empty;
    protected override string ToTypedBuilderFormatString => string.Empty;
}