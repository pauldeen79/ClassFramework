namespace ClassFramework.Pipelines;

public class ResultDictionaryBuilder<T>
{
    private readonly Dictionary<string, Func<Result<T>>> _resultset = new();

    public ResultDictionaryBuilder<T> Add(string name, Func<Result<T>> value)
    {
        _resultset.Add(name, value);
        return this;
    }

    public ResultDictionaryBuilder<T> AddRange(string nameFormatString, Func<IEnumerable<Result<T>>> value)
    {
        value = ArgumentGuard.IsNotNull(value, nameof(value));

        var counter = 0;
        foreach (var item in value())
        {
            var name = string.Format(nameFormatString, counter);
            Add(name, () => item);
            // For now, make it fail fast: stop on first error (but it still gets added to the results, so you can simply check for the first error)
            if (!item.IsSuccessful())
            {
                break;
            }
            counter++;
        }

        return this;
    }

    public Dictionary<string, Result<T>> Build()
    {
        var results = new Dictionary<string, Result<T>>();

        foreach (var item in _resultset)
        {
            var result = item.Value();
            results.Add(item.Key, result);
            if (!result.IsSuccessful())
            {
                break;
            }
        }

        return results;
    }
}
