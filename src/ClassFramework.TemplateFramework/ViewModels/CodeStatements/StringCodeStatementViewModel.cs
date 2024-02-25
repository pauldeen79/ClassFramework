namespace ClassFramework.TemplateFramework.ViewModels.CodeStatements;

public class StringCodeStatementViewModel : CodeStatementViewModelBase<StringCodeStatement>
{
    public StringCodeStatementViewModel(ICsharpExpressionCreator csharpExpressionCreator) : base(csharpExpressionCreator)
    {
    }

    public string? Statement => GetModel().Statement;
}
