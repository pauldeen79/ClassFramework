namespace ClassFramework.Pipelines.Interface.Components;

public class GenericsComponent : IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>
{
    public Task<Result> ExecuteAsync(GenerateInterfaceCommand context, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.AddGenericTypeArguments(context.SourceModel.GenericTypeArguments);
            response.AddGenericTypeArgumentConstraints(context.SourceModel.GenericTypeArgumentConstraints);

            return Result.Success();
        }, token);
}
