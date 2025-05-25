namespace ClassFramework.Pipelines.Extensions;

public static class ExpressionEvaluatorExtensions
{
    //Task<Result<object?>> EvaluateAsync(ExpressionEvaluatorContext context, CancellationToken token);
    public static Task<Result<GenericFormattableString>> Parse(this IExpressionEvaluator instance, string formatString, IFormatProvider formatProvider, object context, CancellationToken token)
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
