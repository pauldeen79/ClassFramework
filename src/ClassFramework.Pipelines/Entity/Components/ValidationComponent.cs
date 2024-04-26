namespace ClassFramework.Pipelines.Entity.Features;

public class ValidationComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext, IConcreteTypeBuilder> Build() => new ValidationComponent();
}

public class ValidationComponent : IPipelineComponent<EntityContext, IConcreteTypeBuilder>
{
    public Task<Result> Process(PipelineContext<EntityContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AllowGenerationWithoutProperties
            && context.Request.SourceModel.Properties.Count == 0
            && !context.Request.Settings.EnableInheritance)
        {
            return Task.FromResult(Result.Invalid("To create an entity class, there must be at least one property"));
        }
        
        return Task.FromResult(Result.Continue());
    }
}
