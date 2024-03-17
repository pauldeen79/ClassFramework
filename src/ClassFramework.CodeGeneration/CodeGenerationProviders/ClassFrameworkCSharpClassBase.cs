namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
#pragma warning disable S125 // Sections of code should not be commented out
public abstract class ClassFrameworkCSharpClassBase : CsharpClassGeneratorPipelineCodeGenerationProviderBase
{
    protected ClassFrameworkCSharpClassBase(ICsharpExpressionDumper csharpExpressionDumper, IPipeline<IConcreteTypeBuilder, BuilderContext> builderPipeline, IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> builderExtensionPipeline, IPipeline<IConcreteTypeBuilder, EntityContext> entityPipeline, IPipeline<IConcreteTypeBuilder, OverrideEntityContext> overrideEntityPipeline, IPipeline<TypeBaseBuilder, ReflectionContext> reflectionPipeline, IPipeline<InterfaceBuilder, InterfaceContext> interfacePipeline) : base(csharpExpressionDumper, builderPipeline, builderExtensionPipeline, entityPipeline, overrideEntityPipeline, reflectionPipeline, interfacePipeline)
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

    protected override IEnumerable<TypenameMappingBuilder> CreateTypenameMappings()
        => base.CreateTypenameMappings().Concat(
        [
            new TypenameMappingBuilder().WithSourceTypeName($"{CodeGenerationRootNamespace}.Models.Domains.ArgumentValidationType").WithTargetTypeName("ClassFramework.Pipelines.Domains.ArgumentValidationType"),
            new TypenameMappingBuilder().WithSourceTypeName($"{CodeGenerationRootNamespace}.Models.Pipelines.INamespaceMapping").WithTargetTypeName("ClassFramework.Pipelines.NamespaceMapping"),
            new TypenameMappingBuilder().WithSourceTypeName("ClassFramework.Pipelines.NamespaceMapping").WithTargetTypeName("ClassFramework.Pipelines.NamespaceMapping")
                .AddMetadata
                (
                    new MetadataBuilder().WithValue("ClassFramework.Pipelines.Builders").WithName(Pipelines.MetadataNames.CustomBuilderNamespace),
                    new MetadataBuilder().WithValue("{TypeName.ClassName}Builder").WithName(Pipelines.MetadataNames.CustomBuilderName),
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].ToBuilder()").WithName(Pipelines.MetadataNames.CustomBuilderSourceExpression),
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].Build()").WithName(Pipelines.MetadataNames.CustomBuilderMethodParameterExpression)
                ),
            new TypenameMappingBuilder().WithSourceTypeName($"{CodeGenerationRootNamespace}.Models.Pipelines.ITypenameMapping").WithTargetTypeName("ClassFramework.Pipelines.TypenameMapping"),
            new TypenameMappingBuilder().WithSourceTypeName("ClassFramework.Pipelines.TypenameMapping").WithTargetTypeName("ClassFramework.Pipelines.TypenameMapping")
                .AddMetadata
                (
                    new MetadataBuilder().WithValue("ClassFramework.Pipelines.Builders").WithName(Pipelines.MetadataNames.CustomBuilderNamespace),
                    new MetadataBuilder().WithValue("{TypeName.ClassName}Builder").WithName(Pipelines.MetadataNames.CustomBuilderName),
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].ToBuilder()").WithName(Pipelines.MetadataNames.CustomBuilderSourceExpression),
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].Build()").WithName(Pipelines.MetadataNames.CustomBuilderMethodParameterExpression)
                ),
            new TypenameMappingBuilder().WithSourceTypeName($"{CodeGenerationRootNamespace}.Models.Pipelines.IAttributeInitializer").WithTargetTypeName("ClassFramework.Pipelines.AttributeInitializer"),
            new TypenameMappingBuilder().WithSourceTypeName("ClassFramework.Pipelines.AttributeInitializer").WithTargetTypeName("ClassFramework.Pipelines.AttributeInitializer")
                .AddMetadata
                (
                    new MetadataBuilder().WithValue("ClassFramework.Pipelines.Builders").WithName(Pipelines.MetadataNames.CustomBuilderNamespace),
                    new MetadataBuilder().WithValue("{TypeName.ClassName}Builder").WithName(Pipelines.MetadataNames.CustomBuilderName),
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].ToBuilder()").WithName(Pipelines.MetadataNames.CustomBuilderSourceExpression),
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].Build()").WithName(Pipelines.MetadataNames.CustomBuilderMethodParameterExpression)
                ),
        ]);
}
#pragma warning restore S125 // Sections of code should not be commented out
