namespace ClassFramework.Domain.Builders;

public partial class PropertyBuilder
{
    partial void SetDefaultValues()
    {
        HasGetter = true;
        HasSetter = true;
    }
}
