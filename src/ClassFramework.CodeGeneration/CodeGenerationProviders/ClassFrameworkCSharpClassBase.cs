using ClassFramework.CodeGeneration.Models.Pipelines;

namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
#pragma warning disable S125 // Sections of code should not be commented out
public abstract class ClassFrameworkCSharpClassBase : CsharpClassGeneratorPipelineCodeGenerationProviderBase
{
    protected ClassFrameworkCSharpClassBase(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, reflectionPipeline, interfacePipeline)
    {
    }

    public override bool RecurseOnDeleteGeneratedFiles => false;
    public override string LastGeneratedFilesFilename => string.Empty;
    public override Encoding Encoding => Encoding.UTF8;

    protected override Type EntityCollectionType => typeof(IReadOnlyCollection<>);
    protected override Type EntityConcreteCollectionType => typeof(ReadOnlyValueCollection<>);
    protected override Type BuilderCollectionType => typeof(ObservableCollection<>);

    protected override string ProjectName => "ClassFramework";
    protected override string CoreNamespace => "ClassFramework.Domain";
    protected override bool CopyAttributes => true;
    protected override bool CopyInterfaces => true;
    protected override bool CreateRecord => true;
    //protected override string ToBuilderFormatString => string.Empty;
    //protected override string ToTypedBuilderFormatString => string.Empty;
    //protected override bool AddCopyConstructor => false;

    protected TypeBase[] GetPipelineModels()
        => GetNonCoreModels($"{CodeGenerationRootNamespace}.Models.Pipelines");

    protected TypeBase[] GetTemplateFrameworkModels()
        => GetNonCoreModels($"{CodeGenerationRootNamespace}.Models.TemplateFramework");

    protected override bool SkipNamespaceOnTypenameMappings(string @namespace)
        => @namespace.In($"{CodeGenerationRootNamespace}.Models.Pipelines",
                         $"{CodeGenerationRootNamespace}.Models.TemplateFramework");

    protected override IEnumerable<TypenameMappingBuilder> CreateAdditionalTypenameMappings()
        => GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == $"{CodeGenerationRootNamespace}.Models.Pipelines" && !GetCustomBuilderTypes().Contains(x.GetEntityClassName()))
            .SelectMany(x => CreateCustomTypenameMappings(x, "ClassFramework.Pipelines", "ClassFramework.Pipelines.Builders"))
            .Concat(
            [
                new TypenameMappingBuilder().WithSourceType(typeof(ArgumentValidationType)).WithTargetTypeName($"ClassFramework.Pipelines.Domains.{nameof(ArgumentValidationType)}"),
                new TypenameMappingBuilder().WithSourceType(typeof(AttributeInitializerDelegate)).WithTargetTypeName($"ClassFramework.Pipelines.{nameof(AttributeInitializerDelegate)}")
            ]);

    private IEnumerable<TypenameMappingBuilder> CreateCustomTypenameMappings(Type modelType, string entityNamespace, string builderNamespace) =>
        [
            new TypenameMappingBuilder()
                .WithSourceType(modelType)
                .WithTargetTypeName($"{entityNamespace}.{modelType.GetEntityClassName()}"),
            new TypenameMappingBuilder()
                .WithSourceTypeName($"{entityNamespace}.{modelType.GetEntityClassName()}")
                .WithTargetTypeName($"{entityNamespace}.{modelType.GetEntityClassName()}")
                .AddMetadata
                (
                    new MetadataBuilder().WithValue(builderNamespace).WithName(Pipelines.MetadataNames.CustomBuilderNamespace),
                    new MetadataBuilder().WithValue("{TypeName.ClassName}Builder").WithName(Pipelines.MetadataNames.CustomBuilderName),
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].ToBuilder()").WithName(Pipelines.MetadataNames.CustomBuilderSourceExpression),
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].Build()").WithName(Pipelines.MetadataNames.CustomBuilderMethodParameterExpression)
                ),
        ];

}
#pragma warning restore S125 // Sections of code should not be commented out
