namespace ClassFramework.Pipelines.Interface.Components;

public class GenericsComponent : IPipelineComponent<InterfaceContext, InterfaceBuilder>
{
    public Task<Result> ExecuteAsync(InterfaceContext context, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            response.AddGenericTypeArguments(context.SourceModel.GenericTypeArguments);
            response.AddGenericTypeArgumentConstraints(context.SourceModel.GenericTypeArgumentConstraints);

            return Result.Success();
        }, token);
}
