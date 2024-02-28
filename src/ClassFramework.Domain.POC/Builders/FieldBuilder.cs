namespace ClassFramework.Domain.Builders;

public partial class FieldBuilder
{
    partial void SetDefaultValues()
    {
        Visibility = Visibility.Private;
    }
}
