namespace ClassFramework.Pipelines;

public class NamedResult<T>
{
    public string Name { get; }
    public T Result { get; }

    public NamedResult(string name, T result)
    {
        Name = name.IsNotNull(nameof(name));
        Result = result.IsNotNull(nameof(result));
    }
}
