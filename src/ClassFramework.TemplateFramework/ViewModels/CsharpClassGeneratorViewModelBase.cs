namespace ClassFramework.TemplateFramework.ViewModels;

public abstract class CsharpClassGeneratorViewModelBase : ICsharpClassGeneratorSettingsContainer, IViewModel
{
    public CsharpClassGeneratorSettings Settings { get; set; } = default!; // will always be injected in CreateModel (root viewmodel) or OnSetContext (child viewmodels) method
}

public abstract class CsharpClassGeneratorViewModelBase<TModel> : CsharpClassGeneratorViewModelBase, IModelContainer<TModel>, ITemplateContextContainer
{
    public TModel Model { get; set; } = default!;
    public ITemplateContext Context { get; set; } = default!; // will always be injected in OnSetContext method

    protected object? ParentModel
        => Context?.ParentContext?.Model;

    public string CreateIndentation(int additionalIndents = 0)
        => new(' ', 4 * (Context.GetIndentCount() + additionalIndents));
}
