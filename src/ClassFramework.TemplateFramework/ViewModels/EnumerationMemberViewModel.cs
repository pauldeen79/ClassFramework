namespace ClassFramework.TemplateFramework.ViewModels;

public class EnumerationMemberViewModel : CsharpClassGeneratorViewModelBase<EnumerationMember>
{
    public EnumerationMemberViewModel(ICsharpExpressionCreator csharpExpressionCreator) : base(csharpExpressionCreator)
    {
    }

    public string ValueExpression
        => GetModel().Value is null
            ? string.Empty
            : $" = {CsharpExpressionCreator.Create(Model!.Value)}";

    public string Name
        => GetModel().Name.Sanitize().GetCsharpFriendlyName();
}
