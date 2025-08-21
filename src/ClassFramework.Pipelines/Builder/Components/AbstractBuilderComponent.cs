namespace ClassFramework.Pipelines.Builder.Components;

public class AbstractBuilderComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.IsBuilderForAbstractEntity)
        {
            return Result.Continue();
        }

        return (await _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request, token)
            .ConfigureAwait(false))
            .OnSuccess(nameResult =>
            {
                context.Request.Builder.WithAbstract();

                if (!context.Request.Settings.IsForAbstractBuilder)
                {
                    var generics = context.Request.SourceModel.GetGenericTypeArgumentsString();
                    var genericsSuffix = string.IsNullOrEmpty(generics)
                        ? string.Empty
                        : $", {context.Request.SourceModel.GetGenericTypeArgumentsString(false)}";

                    context.Request.Builder
                        .AddGenericTypeArguments("TBuilder", "TEntity")
                        .AddGenericTypeArgumentConstraints($"where TEntity : {context.Request.SourceModel.GetFullName()}{generics}")
                        .AddGenericTypeArgumentConstraints($"where TBuilder : {nameResult.Value}<TBuilder, TEntity{genericsSuffix}>")
                        .WithAbstract();
                }

                return Result.NoContent<GenericFormattableString>();
            });
    }
}
