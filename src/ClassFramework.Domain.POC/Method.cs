namespace ClassFramework.Domain;

public partial class Method
{
    public bool IsInterfaceMethod()
        => Name.StartsWith("I", StringComparison.Ordinal)
        && Name.Length > 1
        && Name.Contains('.');
}
