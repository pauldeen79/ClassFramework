namespace ClassFramework.TemplateFramework.ViewModels.CodeStatements;

public class StringCodeStatementViewModel : CodeStatementViewModelBase<StringCodeStatement>
{
    public string? Statement => Model.Statement;
}
