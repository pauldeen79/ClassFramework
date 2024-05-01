namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
#pragma warning disable S125 // Sections of code should not be commented out
public abstract class ClassFrameworkCSharpClassBase : CsharpClassGeneratorPipelineCodeGenerationProviderBase
{
    protected ClassFrameworkCSharpClassBase(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
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
    protected override bool GenerateMultipleFiles => false;
    //protected override string ToBuilderFormatString => string.Empty;
    //protected override string ToTypedBuilderFormatString => string.Empty;
    //protected override bool AddCopyConstructor => false;

    protected async Task<TypeBase[]> GetPipelineModels()
        => await GetNonCoreModels($"{CodeGenerationRootNamespace}.Models.Pipelines").ConfigureAwait(false);

    protected async Task<TypeBase[]> GetTemplateFrameworkModels()
        => await GetNonCoreModels($"{CodeGenerationRootNamespace}.Models.TemplateFramework").ConfigureAwait(false);

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
                new TypenameMappingBuilder().WithSourceType(typeof(AttributeInitializerDelegate)).WithTargetTypeName($"ClassFramework.Pipelines.{nameof(AttributeInitializerDelegate)}"),
                new TypenameMappingBuilder().WithSourceType(typeof(CopyMethodPredicate)).WithTargetTypeName($"ClassFramework.Pipelines.{nameof(CopyMethodPredicate)}"),
                new TypenameMappingBuilder().WithSourceType(typeof(InheritanceComparisonDelegate)).WithTargetTypeName($"ClassFramework.Pipelines.{nameof(InheritanceComparisonDelegate)}"),
                new TypenameMappingBuilder().WithSourceType(typeof(ReflectionInheritanceComparisonDelegate)).WithTargetTypeName($"ClassFramework.Pipelines.{nameof(ReflectionInheritanceComparisonDelegate)}"),
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
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].ToBuilder()[ForcedNullableSuffix]").WithName(Pipelines.MetadataNames.CustomBuilderSourceExpression),
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].Build()[ForcedNullableSuffix]").WithName(Pipelines.MetadataNames.CustomBuilderMethodParameterExpression)
                ),
        ];
}
#pragma warning restore S125 // Sections of code should not be commented out
