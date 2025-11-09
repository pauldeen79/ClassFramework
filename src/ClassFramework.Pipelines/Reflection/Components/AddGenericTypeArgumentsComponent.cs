namespace ClassFramework.Pipelines.Reflection.Components;

public class AddGenericTypeArgumentsComponent : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand context, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.AddGenericTypeArguments(context.SourceModel.GetGenericTypeArgumentTypeNames());

            return Result.Success();
        }, token);
}
