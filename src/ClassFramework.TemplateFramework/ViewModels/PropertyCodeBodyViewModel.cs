namespace ClassFramework.TemplateFramework.ViewModels;

public class PropertyCodeBodyViewModel : CsharpClassGeneratorViewModelBase<PropertyCodeBodyModel>
{
    public PropertyCodeBodyViewModel(ICsharpExpressionCreator csharpExpressionCreator) : base(csharpExpressionCreator)
    {
    }

    public string Modifiers
        => GetModel().Modifiers;

    public string Verb
        => GetModel().Verb;

    public bool OmitCode
        => GetModel().OmitCode;

    public IEnumerable<CodeStatementBase> CodeStatementModels
        => GetModel().CodeStatementModels;
}
