namespace ClassFramework.TemplateFramework.CodeGenerationProviders;

public abstract class CsharpClassGeneratorCodeGenerationProviderBase : ICodeGenerationProvider
{
    protected CsharpClassGeneratorCodeGenerationProviderBase(ICsharpExpressionDumper csharpExpressionDumper)
    {
        Guard.IsNotNull(csharpExpressionDumper);

        CsharpExpressionDumper = csharpExpressionDumper;
    }

    protected ICsharpExpressionDumper CsharpExpressionDumper { get; }

    public abstract string Path { get; }
    public abstract bool RecurseOnDeleteGeneratedFiles { get; }
    public abstract string LastGeneratedFilesFilename { get; }
    public abstract Encoding Encoding { get; }

    public object? CreateAdditionalParameters() => null;

    public Type GetGeneratorType() => typeof(CsharpClassGenerator);

    public abstract IEnumerable<TypeBase> Model { get; }
    public abstract CsharpClassGeneratorSettings Settings { get; }

    public object? CreateModel()
        => new CsharpClassGeneratorViewModel(CsharpExpressionDumper)
        {
            Model = Model,
            Settings = Settings
            //Context is filled in base class, on the property setter of Context (propagated to Model)
        };

    protected virtual string CurrentNamespace => Path.Replace('/', '.');
}
