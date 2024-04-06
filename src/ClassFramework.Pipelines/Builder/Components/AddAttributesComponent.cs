namespace ClassFramework.Pipelines.Builder.Features;

public class AddAttributesComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build()
        => new AddAttributesComponent();
}

public class AddAttributesComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.AddAttributes(context.Context.GetAtributes(context.Context.SourceModel.Attributes));

        return Result.Continue<IConcreteTypeBuilder>();
    }
}
