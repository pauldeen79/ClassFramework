namespace ClassFramework.TemplateFramework.ViewModels;

public class EnumerationMemberViewModel(ICsharpExpressionDumper csharpExpressionDumper) : CsharpClassGeneratorViewModelBase<EnumerationMember>
{
    public string ValueExpression
        => Model.Value is null
            ? string.Empty
            : $" = {csharpExpressionDumper.Dump(Model!.Value)}";

    public string Name
        => Model.Name.Sanitize().GetCsharpFriendlyName();
}
