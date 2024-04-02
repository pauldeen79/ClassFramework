namespace ClassFramework.Domain;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class CsharpTypeNameAttribute : System.Attribute
{
    public string TypeName { get; }

    public CsharpTypeNameAttribute(string typeName)
    {
        TypeName = typeName;
    }
}
