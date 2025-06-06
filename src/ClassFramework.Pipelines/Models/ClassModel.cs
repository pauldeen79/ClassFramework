namespace ClassFramework.Pipelines.Models;

public class ClassModel
{
    private readonly Type? _type;
    private readonly TypeBase? _typeBase;

    public ClassModel(Type type)
    {
        ArgumentGuard.IsNotNull(type, nameof(type));

        _type = type;
    }

    public ClassModel(TypeBase typeBase)
    {
        ArgumentGuard.IsNotNull(typeBase, nameof(typeBase));
        _typeBase = typeBase;
    }

    public string Name => _type?.Name ?? _typeBase!.Name;
    public string Namespace => _type?.Namespace ?? _typeBase!.Namespace;
    public string[] GetGenericTypeArguments() => _type?.GetGenericArguments().Select(x => x.FullName).ToArray() ?? _typeBase!.GenericTypeArguments.ToArray();
}
