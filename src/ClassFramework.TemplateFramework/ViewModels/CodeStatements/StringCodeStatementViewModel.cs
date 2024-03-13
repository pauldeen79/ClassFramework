namespace ClassFramework.TemplateFramework.ViewModels.CodeStatements;

public class StringCodeStatementViewModel : CodeStatementViewModelBase<StringCodeStatement>
{
    public StringCodeStatementViewModel(ICsharpExpressionDumper csharpExpressionDumper) : base(csharpExpressionDumper)
    {
    }

    public string? Statement => GetModel().Statement;
}
