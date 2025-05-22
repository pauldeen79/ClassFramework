namespace ClassFramework.Pipelines.Extensions;

public static class ExpressionEvaluatorExtensions
{
    //Task<Result<object?>> EvaluateAsync(ExpressionEvaluatorContext context, CancellationToken token);
    public static Task<Result<GenericFormattableString>> Parse(this IExpressionEvaluator instance, string formatString, IFormatProvider formatProvider, object context)
    {
    }
}
