namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
#pragma warning disable S125 // Sections of code should not be commented out
public abstract class ClassFrameworkCSharpClassBase(IPipelineService pipelineService) : CsharpClassGeneratorPipelineCodeGenerationProviderBase(pipelineService)
{
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
    protected override bool EnableGlobalUsings => true;
    //protected override string ToBuilderFormatString => string.Empty;
    //protected override string ToTypedBuilderFormatString => string.Empty;
    //protected override bool AddCopyConstructor => false;

    protected Task<Result<IEnumerable<TypeBase>>> GetPipelineModelsAsync()
        => GetNonCoreModelsAsync($"{CodeGenerationRootNamespace}.Models.Pipelines");

    protected Task<Result<IEnumerable<TypeBase>>> GetTemplateFrameworkModelsAsync()
        => GetNonCoreModelsAsync($"{CodeGenerationRootNamespace}.Models.TemplateFramework");

    protected override string[] GetAdditionalSkippedNamespacesOnTypenameMappings() =>
    [
        $"{CodeGenerationRootNamespace}.Models.Pipelines",
        $"{CodeGenerationRootNamespace}.Models.TemplateFramework",
    ];

    protected override IEnumerable<TypenameMappingBuilder> GetAdditionalTypenameMappings()
        => GetType().Assembly.GetTypes()
            .Where(x => x.IsInterface && x.Namespace == $"{CodeGenerationRootNamespace}.Models.Pipelines" && !GetCustomBuilderTypes().Contains(x.GetEntityClassName()))
            .SelectMany(x => CreateCustomTypenameMappings(x, "ClassFramework.Pipelines", "ClassFramework.Pipelines.Builders"))
            .Concat(
            [
                new TypenameMappingBuilder(typeof(ArgumentValidationType), $"ClassFramework.Pipelines.Domains.{nameof(ArgumentValidationType)}"),
                new TypenameMappingBuilder(typeof(IEquatableItemType), $"ClassFramework.Pipelines.Domains.{nameof(IEquatableItemType)}"),
                new TypenameMappingBuilder(typeof(Models.Pipelines.AttributeInitializerDelegate), $"ClassFramework.Pipelines.{nameof(Models.Pipelines.AttributeInitializerDelegate)}"),
                new TypenameMappingBuilder(typeof(Models.Pipelines.CopyMethodPredicate), $"ClassFramework.Pipelines.{nameof(Models.Pipelines.CopyMethodPredicate)}"),
                new TypenameMappingBuilder(typeof(Models.Pipelines.InheritanceComparisonDelegate), $"ClassFramework.Pipelines.{nameof(Models.Pipelines.InheritanceComparisonDelegate)}"),
                new TypenameMappingBuilder(typeof(Models.Pipelines.ReflectionInheritanceComparisonDelegate), $"ClassFramework.Pipelines.{nameof(Models.Pipelines.ReflectionInheritanceComparisonDelegate)}"),
            ]);

    private static IEnumerable<TypenameMappingBuilder> CreateCustomTypenameMappings(Type modelType, string entityNamespace, string buildersNamespace) =>
        [
            new TypenameMappingBuilder(modelType, $"{entityNamespace}.{modelType.GetEntityClassName()}"),
            new TypenameMappingBuilder($"{entityNamespace}.{modelType.GetEntityClassName()}", $"{entityNamespace}.{modelType.GetEntityClassName()}")
                .AddMetadata(CreateTypenameMappingMetadata(buildersNamespace)),
        ];
}
#pragma warning restore S125 // Sections of code should not be commented out
