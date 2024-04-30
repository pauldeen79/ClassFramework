namespace ClassFramework.TemplateFramework.ViewModels;

public class EnumerationViewModel : AttributeContainerViewModelBase<Enumeration>
{
    public string Modifiers
        => GetModel().GetModifiers(Settings.CultureInfo);

    public string Name
        => GetModel().Name.Sanitize().GetCsharpFriendlyName();

    public IEnumerable<EnumerationMember> Members
        => GetModel().Members;
}
