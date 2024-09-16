namespace ClassFramework.TemplateFramework.CodeGenerationProviders;

public abstract class CsharpClassGeneratorCodeGenerationProviderBase : ICodeGenerationProvider
{
    public abstract string Path { get; }
    public abstract bool RecurseOnDeleteGeneratedFiles { get; }
    public abstract string LastGeneratedFilesFilename { get; }
    public abstract Encoding Encoding { get; }

    public Task<object?> CreateAdditionalParameters() => Task.FromResult(default(object?));

    public Type GetGeneratorType() => typeof(CsharpClassGenerator);

    public async Task<object?> CreateModel()
        => new CsharpClassGeneratorViewModel
        {
            Model = await GetModel(),
            Settings = Settings
            //Context is filled in base class, on the property setter of Context (propagated to Model)
        };

    public IGenerationEnvironment CreateGenerationEnvironment()
        => Settings.GenerateMultipleFiles
            ? new MultipleStringContentBuilderEnvironment()
            : new StringBuilderEnvironment();

    public abstract Task<IEnumerable<TypeBase>> GetModel();
    public abstract CsharpClassGeneratorSettings Settings { get; }

    protected virtual string CurrentNamespace => Path.Replace('/', '.');
}
