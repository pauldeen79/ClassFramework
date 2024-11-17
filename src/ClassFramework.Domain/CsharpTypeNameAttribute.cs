namespace ClassFramework.Domain;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class CsharpTypeNameAttribute(string typeName) : System.Attribute
{
    public string TypeName { get; } = typeName;
}
