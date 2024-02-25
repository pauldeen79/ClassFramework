namespace ClassFramework.CsharpExpressionCreator;

public class DefaultCsharpExpressionCreator : ICsharpExpressionCreator
{
    private readonly ICsharpExpressionDumper _dumper;

    public DefaultCsharpExpressionCreator(ICsharpExpressionDumper dumper)
        => _dumper = dumper.IsNotNull(nameof(dumper));

    public string Create(object? instance)
        => _dumper.Dump(instance, instance?.GetType());
}
