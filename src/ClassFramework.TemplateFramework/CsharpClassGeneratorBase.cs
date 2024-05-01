namespace ClassFramework.TemplateFramework;

public abstract class CsharpClassGeneratorBase<TModel> : TemplateBase, IModelContainer<TModel>
    where TModel : ICsharpClassGeneratorSettingsContainer, IMediatorContainer
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

    public TModel? Model { get; set; }

    protected void RenderChildTemplateByModel(object model, IGenerationEnvironment generationEnvironment)
    {
        Guard.IsNotNull(Context);
        Context.Engine.RenderChildTemplate(model, generationEnvironment, Context, new TemplateByModelIdentifier(model));
    }

    protected void RenderChildTemplatesByModel(IEnumerable models, StringBuilder builder)
    {
        RenderChildTemplatesByModel(models, new StringBuilderEnvironment(builder));
    }

    protected void RenderChildTemplatesByModel(IEnumerable models, IGenerationEnvironment generationEnvironment)
    {
        Guard.IsNotNull(Context);
        Context.Engine.RenderChildTemplates(models, generationEnvironment, Context, model => new TemplateByModelIdentifier(model));
    }
}
