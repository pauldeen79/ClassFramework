namespace ClassFramework.Pipelines.Entity.Components;

public class AddAttributesComponent : IPipelineComponent<EntityContext, ClassBuilder>
{
    public Task<Result> ExecuteAsync(EntityContext context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            response.AddAttributes(context.GetAtributes(context.SourceModel.Attributes));

            return Result.Success();
        }, token);
}
