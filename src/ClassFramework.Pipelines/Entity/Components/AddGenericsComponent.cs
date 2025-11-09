namespace ClassFramework.Pipelines.Entity.Components;

public class AddGenericsComponent : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateEntityCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response
                .AddGenericTypeArguments(context.SourceModel.GenericTypeArguments)
                .AddGenericTypeArgumentConstraints(context.SourceModel.GenericTypeArgumentConstraints);

            return Result.Success();
        }, token);
}
