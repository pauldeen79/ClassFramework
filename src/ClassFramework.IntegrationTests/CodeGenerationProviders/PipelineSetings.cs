//using ClassFramework.Domain.Builders.Extensions;
//using ClassFramework.Pipelines;

//namespace ClassFramework.IntegrationTests.CodeGenerationProviders;

//public class PipelineSetings : TestCodeGenerationProviderBase
//{
//    public PipelineSetings(ICsharpExpressionCreator csharpExpressionCreator, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<IConcreteTypeBuilder, OverrideEntityContext> overrideEntityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionCreator, builderPipeline, builderExtensionPipeline, entityPipeline, overrideEntityPipeline, reflectionPipeline, interfacePipeline)
//    {
//    }

//    public override string Path => "ClassFramework.Pipelines";

//    public override IEnumerable<TypeBase> Model => new[]
//    {
//        new ClassBuilder()
//            .WithName("PipelineSettings")
//            .AddProperties(typeof(MetadataNames).Assembly.GetExportedTypes()
//                .Where(x => x.Name.EndsWith("Settings") && !x.IsAbstract)
//                .SelectMany(x => x.GetProperties())
//                .Where(x => !x.PropertyType.Namespace?.StartsWith("ClassFramework.Pipelines") == true)
//                .GroupBy(x => x.Name)
//                .OrderBy(x => x.Key)
//                .Select(x => new PropertyBuilder().WithName(x.Key).WithType(x.First().PropertyType).WithHasSetter(false)))
//                .Build()
//    };
//}
