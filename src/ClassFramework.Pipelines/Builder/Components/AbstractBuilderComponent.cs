namespace ClassFramework.Pipelines.Builder.Components;

public class AbstractBuilderComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        if (!command.IsBuilderForAbstractEntity)
        {
            return Result.Continue();
        }

        return (await _evaluator.EvaluateInterpolatedStringAsync(command.Settings.BuilderNameFormatString, command.FormatProvider, command, token)
            .ConfigureAwait(false))
            .OnSuccess(nameResult =>
            {
                response.WithAbstract();

                if (!command.Settings.IsForAbstractBuilder)
                {
                    var generics = command.SourceModel.GetGenericTypeArgumentsString();
                    var genericsSuffix = string.IsNullOrEmpty(generics)
                        ? string.Empty
                        : $", {command.SourceModel.GetGenericTypeArgumentsString(false)}";

                    response
                        .AddGenericTypeArguments("TBuilder", "TEntity")
                        .AddGenericTypeArgumentConstraints($"where TEntity : {command.SourceModel.GetFullName()}{generics}")
                        .AddGenericTypeArgumentConstraints($"where TBuilder : {nameResult.Value}<TBuilder, TEntity{genericsSuffix}>")
                        .WithAbstract();
                }
            });
    }
}
