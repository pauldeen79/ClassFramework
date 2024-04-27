namespace ClassFramework.Pipelines.Entity.Components;

public class AddAttributesComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext, IConcreteTypeBuilder> Build()
        => new AddAttributesComponent();
}

public class AddAttributesComponent : IPipelineComponent<EntityContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<EntityContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Response.AddAttributes(context.Request.GetAtributes(context.Request.SourceModel.Attributes));

        return Task.FromResult(Result.Continue());
    }
}
