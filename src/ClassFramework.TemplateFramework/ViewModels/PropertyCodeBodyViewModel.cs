namespace ClassFramework.TemplateFramework.ViewModels;

public class PropertyCodeBodyViewModel : CsharpClassGeneratorViewModelBase<PropertyCodeBodyModel>
{
    public string Modifiers
        => Model.Modifiers;

    public string Verb
        => Model.Verb;

    public bool OmitCode
        => Model.OmitCode;

    public IEnumerable<CodeStatementBase> CodeStatements
        => Model.CodeStatementModels;
}
