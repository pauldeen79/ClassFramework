﻿namespace ClassFramework.TemplateFramework.CodeGenerationProviders;

public abstract class CsharpClassGeneratorCodeGenerationProviderBase : ICodeGenerationProvider
{
    protected CsharpClassGeneratorCodeGenerationProviderBase(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper)
    {
        Guard.IsNotNull(mediator);
        Guard.IsNotNull(csharpExpressionDumper);

        CsharpExpressionDumper = csharpExpressionDumper;
        Mediator = mediator;
    }

    protected ICsharpExpressionDumper CsharpExpressionDumper { get; }
    protected IMediator Mediator { get; }

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
            ? new MultipleContentBuilderEnvironment()
            : new StringBuilderEnvironment();

    public abstract Task<IEnumerable<TypeBase>> GetModel();
    public abstract CsharpClassGeneratorSettings Settings { get; }


    protected virtual string CurrentNamespace => Path.Replace('/', '.');
}
