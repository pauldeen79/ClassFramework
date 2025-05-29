namespace ClassFramework.Pipelines.Extensions;

public static class ExpressionEvaluatorExtensions
{
    public static Task<Result<GenericFormattableString>> EvaluateAsync(this IExpressionEvaluator instance, string formatString, IFormatProvider formatProvider, object context, CancellationToken token)
        => instance.EvaluateTypedAsync<GenericFormattableString>
        (
            new ExpressionEvaluatorContext
            (
                $"$\"{formatString}\"",
                new ExpressionEvaluatorSettingsBuilder().WithFormatProvider(formatProvider),
                instance,
                new Dictionary<string, Task<Result<object?>>> { { "context", Task.FromResult(Result.Success<object?>(context)) } }
            ), token
        );
}
