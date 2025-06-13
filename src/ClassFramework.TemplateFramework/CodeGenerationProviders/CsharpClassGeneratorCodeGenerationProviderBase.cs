namespace ClassFramework.TemplateFramework.CodeGenerationProviders;

public abstract class CsharpClassGeneratorCodeGenerationProviderBase : ICodeGenerationProvider
{
    public abstract string Path { get; }
    public abstract bool RecurseOnDeleteGeneratedFiles { get; }
    public abstract string LastGeneratedFilesFilename { get; }
    public abstract Encoding Encoding { get; }

    public Task<Result<object?>> CreateAdditionalParametersAsync(CancellationToken cancellationToken) => Task.FromResult(Result.Success(default(object?)));

    public Type GetGeneratorType() => typeof(CsharpClassGenerator);

    public async Task<Result<object?>> CreateModelAsync(CancellationToken cancellationToken)
    {
        var modelResult = await GetModel(cancellationToken).ConfigureAwait(false);
        if (!modelResult.IsSuccessful())
        {
            return modelResult.TryCast<object?>();
        }

        return Result.Success<object?>(new CsharpClassGeneratorViewModel
        {
            Model = modelResult.Value,
            Settings = Settings
            //Context is filled in base class, on the property setter of Context (propagated to Model)
        });
    }

    public IGenerationEnvironment CreateGenerationEnvironment()
        => Settings.GenerateMultipleFiles
            ? new MultipleStringContentBuilderEnvironment()
            : new StringBuilderEnvironment();

    public abstract Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken);
    public abstract CsharpClassGeneratorSettings Settings { get; }

    protected virtual string CurrentNamespace => Path.Replace('/', '.');
}
