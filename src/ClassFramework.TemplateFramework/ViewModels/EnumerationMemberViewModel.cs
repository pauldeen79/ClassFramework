namespace ClassFramework.TemplateFramework.ViewModels;

public class EnumerationMemberViewModel : CsharpClassGeneratorViewModelBase<EnumerationMember>
{
    public string ValueExpression
        => GetModel().Value is null
            ? string.Empty
            : $" = {GetCsharpExpression(Model!.Value)}";

    public string Name
        => GetModel().Name.Sanitize().GetCsharpFriendlyName();
}
