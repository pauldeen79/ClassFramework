namespace ClassFramework.TemplateFramework;

public abstract class CsharpClassGeneratorBase<TModel> : TemplateBase, IModelContainer<TModel>
    where TModel : ICsharpClassGeneratorSettingsContainer
{
    protected override void OnSetContext(ITemplateContext value)
    {
        Guard.IsNotNull(value);

        if (Model is ITemplateContextContainer container)
        {
            // Copy Context from template context to ViewModel
            container.Context = value;
        }

        if (Model is not null && Model.Settings is null)
        {
            // Copy Settings from template context to ViewModel
            Model.Settings = value.GetCsharpClassGeneratorSettings()
                ?? throw new InvalidOperationException("Could not get Settings from context");
        }
    }

    public TModel Model { get; set; } = default!;

    protected async Task<Result> RenderChildTemplateByModelAsync(object model, IGenerationEnvironment generationEnvironment, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(Context);
        return await Context.Engine.RenderChildTemplateAsync(model, generationEnvironment, Context, new TemplateByModelIdentifier(model), cancellationToken).ConfigureAwait(false);
    }

    protected Task<Result> RenderChildTemplatesByModelAsync(IEnumerable models, StringBuilder builder, CancellationToken cancellationToken)
        => RenderChildTemplatesByModelAsync(models, new StringBuilderEnvironment(builder), cancellationToken);

    protected async Task<Result> RenderChildTemplatesByModelAsync(IEnumerable models, IGenerationEnvironment generationEnvironment, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(Context);
        return await Context.Engine.RenderChildTemplatesAsync(models, generationEnvironment, Context, model => new TemplateByModelIdentifier(model), cancellationToken).ConfigureAwait(false);
    }
}
