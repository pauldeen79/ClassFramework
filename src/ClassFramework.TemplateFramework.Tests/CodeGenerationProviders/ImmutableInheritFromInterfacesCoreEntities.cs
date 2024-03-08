namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesCoreEntities : ImmutableInheritFromInterfacesCSharpClassBase
{
    public ImmutableInheritFromInterfacesCoreEntities(ICsharpExpressionCreator csharpExpressionCreator, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<IConcreteTypeBuilder, OverrideEntityContext> overrideEntityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionCreator, builderPipeline, builderExtensionPipeline, entityPipeline, overrideEntityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override IEnumerable<TypeBase> Model => GetEntities(GetCoreModels().Select(x => x.ToBuilder().AddInterfaces($"Test.Domain.Abstractions.{x.Name}").Build()).ToArray(), "Test.Domain");

    public override string Path => "Test.Domain";
}
