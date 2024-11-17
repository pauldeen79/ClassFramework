namespace ClassFramework.TemplateFramework.ViewModels;

public class EnumerationMemberViewModel(ICsharpExpressionDumper csharpExpressionDumper) : CsharpClassGeneratorViewModelBase<EnumerationMember>
{
    public string ValueExpression
        => GetModel().Value is null
            ? string.Empty
            : $" = {csharpExpressionDumper.Dump(Model!.Value)}";

    public string Name
        => GetModel().Name.Sanitize().GetCsharpFriendlyName();
}
