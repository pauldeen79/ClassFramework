namespace ClassFramework.TemplateFramework.ViewModels;

public class EnumerationViewModel : AttributeContainerViewModelBase<Enumeration>
{
    public string Modifiers
        => Model.GetModifiers(Settings.CultureInfo);

    public string Name
        => Model.Name.Sanitize().GetCsharpFriendlyName();

    public IEnumerable<EnumerationMember> Members
        => Model.Members;
}
