namespace ClassFramework.Pipelines.Entity.Features;

public class AddAttributesComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build()
        => new AddAttributesComponent();
}

public class AddAttributesComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.AddAttributes(context.Context.GetAtributes(context.Context.SourceModel.Attributes));

        return Result.Continue<IConcreteTypeBuilder>();
    }
}
