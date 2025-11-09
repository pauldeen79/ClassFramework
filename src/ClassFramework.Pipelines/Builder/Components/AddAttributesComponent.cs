namespace ClassFramework.Pipelines.Builder.Components;

public class AddAttributesComponent : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateBuilderCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.AddAttributes(context.GetAtributes(context.SourceModel.Attributes));

            return Result.Success();
        }, token);
}
