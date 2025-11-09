namespace ClassFramework.Pipelines.Entity.Components;

public class AddAttributesComponent : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateEntityCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.AddAttributes(context.GetAtributes(context.SourceModel.Attributes));

            return Result.Success();
        }, token);
}
