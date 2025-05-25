namespace ClassFramework.Pipelines.Builder.Components;

public class AbstractBuilderComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.IsBuilderForAbstractEntity)
        {
            var nameResult = await _evaluator.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request, token).ConfigureAwait(false);
            if (!nameResult.IsSuccessful())
            {
                return nameResult;
            }

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
        }

        return Result.Success();
    }
}
