namespace ClassFramework.Pipelines.Builder.Features;

public class AddAttributesComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<BuilderContext, IConcreteTypeBuilder> Build()
        => new AddAttributesComponent();
}

public class AddAttributesComponent : IPipelineComponent<BuilderContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<BuilderContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Response.AddAttributes(context.Request.GetAtributes(context.Request.SourceModel.Attributes));

        return Task.FromResult(Result.Continue());
    }
}
