namespace ClassFramework.Pipelines.Builder.Components;

public class AbstractBuilderComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.IsBuilderForAbstractEntity /*&& context.Request.IsAbstractBuilder*/)
        {
            var nameResult = _formattableStringParser.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request);
            if (!nameResult.IsSuccessful())
            {
                return Task.FromResult<Result>(nameResult);
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

        return Task.FromResult(Result.Success());
    }
}
