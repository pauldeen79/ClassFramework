namespace ClassFramework.TemplateFramework.ViewModels;

public class EnumerationMemberViewModel : CsharpClassGeneratorViewModelBase<EnumerationMember>
{
    public EnumerationMemberViewModel(ICsharpExpressionDumper csharpExpressionDumper) : base(csharpExpressionDumper)
    {
    }

    public string ValueExpression
        => GetModel().Value is null
            ? string.Empty
            : $" = {CsharpExpressionDumper.Dump(Model!.Value)}";

    public string Name
        => GetModel().Name.Sanitize().GetCsharpFriendlyName();
}
