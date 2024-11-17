namespace ClassFramework.Pipelines;

public class NamedResult<T>(string name, T result)
{
    public string Name { get; } = name.IsNotNull(nameof(name));
    public T Result { get; } = result.IsNotNull(nameof(result));
}
