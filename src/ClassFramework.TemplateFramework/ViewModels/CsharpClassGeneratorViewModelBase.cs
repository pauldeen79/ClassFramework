namespace ClassFramework.TemplateFramework.ViewModels;

public abstract class CsharpClassGeneratorViewModelBase : ICsharpClassGeneratorSettingsContainer, IViewModel
{
    public CsharpClassGeneratorSettings Settings { get; set; } = default!; // will always be injected in CreateModel (root viewmodel) or OnSetContext (child viewmodels) method

    protected CsharpClassGeneratorSettings GetSettings()
    {
        Guard.IsNotNull(Settings);

        return Settings;
    }
}

public abstract class CsharpClassGeneratorViewModelBase<TModel> : CsharpClassGeneratorViewModelBase, IModelContainer<TModel>, ITemplateContextContainer
{
    protected CsharpClassGeneratorViewModelBase(ICsharpExpressionDumper csharpExpressionDumper)
    {
        Guard.IsNotNull(csharpExpressionDumper);

        CsharpExpressionDumper = csharpExpressionDumper;
    }

    public TModel? Model { get; set; }
    public ICsharpExpressionDumper CsharpExpressionDumper { get; set; }
    public ITemplateContext Context { get; set; } = default!; // will always be injected in OnSetContext method

    protected ITemplateContext GetContext()
    {
        Guard.IsNotNull(Context);

        return Context;
    }

    protected TModel GetModel()
    {
        Guard.IsNotNull(Model);

        return Model;
    }

    protected object? GetParentModel()
        => GetContext().ParentContext?.Model;

    public string CreateIndentation(int additionalIndents = 0)
        => new string(' ', 4 * (GetContext().GetIndentCount() + additionalIndents));
}
