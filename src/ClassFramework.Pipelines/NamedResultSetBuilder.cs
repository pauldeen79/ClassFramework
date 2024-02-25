namespace ClassFramework.Pipelines;

public class NamedResultSetBuilder<T>
{
    private readonly List<NamedResult<Func<Result<T>>>> _resultset = new();

    public void Add(string name, Func<Result<T>> value) => _resultset.Add(new(name, value));

    public void AddRange(string name, Func<IEnumerable<Result<T>>> value) => _resultset.AddRange(value.IsNotNull(nameof(value)).Invoke().TakeWhileWithFirstNonMatching(x => x.IsSuccessful()).Select(x => new NamedResult<Func<Result<T>>>(name, () => x)));

    public NamedResult<Result<T>>[] Build()
        => _resultset
            .Select(x => new NamedResult<Result<T>>(x.Name, x.Result()))
            .TakeWhileWithFirstNonMatching(x => x.Result.IsSuccessful())
            .ToArray();
}
