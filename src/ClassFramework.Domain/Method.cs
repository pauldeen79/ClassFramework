namespace ClassFramework.Domain;

public partial record Method
{
    public bool IsInterfaceMethod()
        => Name.StartsWith("I", StringComparison.Ordinal)
        && Name.Length > 1
        && Name.Contains('.');
}
