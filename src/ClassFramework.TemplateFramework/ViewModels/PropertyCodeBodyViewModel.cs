namespace ClassFramework.TemplateFramework.ViewModels;

public class PropertyCodeBodyViewModel : CsharpClassGeneratorViewModelBase<PropertyCodeBodyModel>
{
    public PropertyCodeBodyViewModel(ICsharpExpressionDumper csharpExpressionDumper) : base(csharpExpressionDumper)
    {
    }

    public string Modifiers
        => GetModel().Modifiers;

    public string Verb
        => GetModel().Verb;

    public bool OmitCode
        => GetModel().OmitCode;

    public IEnumerable<CodeStatementBase> CodeStatements
        => GetModel().CodeStatementModels;
}
