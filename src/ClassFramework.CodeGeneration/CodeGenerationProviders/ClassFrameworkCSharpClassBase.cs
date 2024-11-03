namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
#pragma warning disable S125 // Sections of code should not be commented out
public abstract class ClassFrameworkCSharpClassBase : CsharpClassGeneratorPipelineCodeGenerationProviderBase
{
    protected ClassFrameworkCSharpClassBase(IPipelineService pipelineService) : base(pipelineService)
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
    protected override bool EnableGlobalUsings => true;
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
                new TypenameMappingBuilder().WithSourceType(typeof(IEquatableItemType)).WithTargetTypeName($"ClassFramework.Pipelines.Domains.{nameof(IEquatableItemType)}"),
                new TypenameMappingBuilder().WithSourceType(typeof(Models.Pipelines.AttributeInitializerDelegate)).WithTargetTypeName($"ClassFramework.Pipelines.{nameof(Models.Pipelines.AttributeInitializerDelegate)}"),
                new TypenameMappingBuilder().WithSourceType(typeof(Models.Pipelines.CopyMethodPredicate)).WithTargetTypeName($"ClassFramework.Pipelines.{nameof(Models.Pipelines.CopyMethodPredicate)}"),
                new TypenameMappingBuilder().WithSourceType(typeof(Models.Pipelines.InheritanceComparisonDelegate)).WithTargetTypeName($"ClassFramework.Pipelines.{nameof(Models.Pipelines.InheritanceComparisonDelegate)}"),
                new TypenameMappingBuilder().WithSourceType(typeof(Models.Pipelines.ReflectionInheritanceComparisonDelegate)).WithTargetTypeName($"ClassFramework.Pipelines.{nameof(Models.Pipelines.ReflectionInheritanceComparisonDelegate)}"),
            ]);

    private static IEnumerable<TypenameMappingBuilder> CreateCustomTypenameMappings(Type modelType, string entityNamespace, string buildersNamespace) =>
        [
            new TypenameMappingBuilder()
                .WithSourceType(modelType)
                .WithTargetTypeName($"{entityNamespace}.{modelType.GetEntityClassName()}"),
            new TypenameMappingBuilder()
                .WithSourceTypeName($"{entityNamespace}.{modelType.GetEntityClassName()}")
                .WithTargetTypeName($"{entityNamespace}.{modelType.GetEntityClassName()}")
                .AddMetadata(CreateTypenameMappingMetadata(buildersNamespace)),
        ];
}
#pragma warning restore S125 // Sections of code should not be commented out
