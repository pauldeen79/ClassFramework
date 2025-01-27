namespace CrossCutting.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public abstract class CrossCuttingCSharpClassBase(IPipelineService pipelineService) : CsharpClassGeneratorPipelineCodeGenerationProviderBase(pipelineService)
{
    public override bool RecurseOnDeleteGeneratedFiles => false;
    public override string LastGeneratedFilesFilename => string.Empty;
    public override Encoding Encoding => Encoding.UTF8;

    protected override Type EntityCollectionType => typeof(IReadOnlyCollection<>);
    protected override Type EntityConcreteCollectionType => typeof(ReadOnlyValueCollection<>);
    protected override Type BuilderCollectionType => typeof(List<>);
    protected override string ProjectName => Constants.ProjectName;
    protected override string CoreNamespace => Constants.Namespaces.UtilitiesParsers; // standard implementation thinks we're using the project name concatenated with '.Domain'
    protected override bool CopyAttributes => true;
    protected override bool CopyInterfaces => true;
    protected override bool CreateRecord => true;
    protected override bool GenerateMultipleFiles => false;
    protected override bool EnableGlobalUsings => true;

    protected override bool IsAbstractType(Type type)
    {
        type = ArgumentGuard.IsNotNull(type, nameof(type));

        if (type.IsInterface && type.Namespace == $"{CodeGenerationRootNamespace}.Models" && type.Name[1..].ReplaceSuffix("`1", string.Empty, StringComparison.Ordinal).In(Constants.Types.FunctionCallArgument, Constants.Types.TypedFunctionCallArgument))
        {
            return true;
        }

        return base.IsAbstractType(type);
    }

    protected override IEnumerable<TypenameMappingBuilder> CreateAdditionalTypenameMappings()
    {
        yield return new TypenameMappingBuilder()
            .WithSourceType(typeof(IFormatProvider))
            .WithTargetType(typeof(IFormatProvider))
            .AddMetadata
            (
                new MetadataBuilder()
                    .WithValue(new Literal($"{typeof(CultureInfo).FullName}.{nameof(CultureInfo.InvariantCulture)}", null))
                    .WithName(ClassFramework.Pipelines.MetadataNames.CustomBuilderDefaultValue)
            );
    }
}
