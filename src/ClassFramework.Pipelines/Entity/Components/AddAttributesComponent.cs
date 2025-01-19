namespace ClassFramework.Pipelines.Entity.Components;

public class AddAttributesComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext> Build()
        => new AddAttributesComponent();
}

public class AddAttributesComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.AddAttributes(context.Request.GetAtributes(context.Request.SourceModel.Attributes));

        return Task.FromResult(Result.Success());
    }
}
