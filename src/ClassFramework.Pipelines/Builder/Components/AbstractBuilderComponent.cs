namespace ClassFramework.Pipelines.Builder.Components;

public class AbstractBuilderComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(BuilderContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        if (!context.IsBuilderForAbstractEntity)
        {
            return Result.Continue();
        }

        return (await _evaluator.EvaluateInterpolatedStringAsync(context.Settings.BuilderNameFormatString, context.FormatProvider, context, token)
            .ConfigureAwait(false))
            .OnSuccess(nameResult =>
            {
                response.WithAbstract();

                if (!context.Settings.IsForAbstractBuilder)
                {
                    var generics = context.SourceModel.GetGenericTypeArgumentsString();
                    var genericsSuffix = string.IsNullOrEmpty(generics)
                        ? string.Empty
                        : $", {context.SourceModel.GetGenericTypeArgumentsString(false)}";

                    response
                        .AddGenericTypeArguments("TBuilder", "TEntity")
                        .AddGenericTypeArgumentConstraints($"where TEntity : {context.SourceModel.GetFullName()}{generics}")
                        .AddGenericTypeArgumentConstraints($"where TBuilder : {nameResult.Value}<TBuilder, TEntity{genericsSuffix}>")
                        .WithAbstract();
                }
            });
    }
}
