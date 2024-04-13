namespace ClassFramework.Pipelines.Entity.Features;

public class ValidationComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build() => new ValidationComponent();
}

public class ValidationComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.AllowGenerationWithoutProperties
            && context.Context.SourceModel.Properties.Count == 0
            && !context.Context.Settings.EnableInheritance)
        {
            return Result.Invalid<IConcreteTypeBuilder>("To create an entity class, there must be at least one property");
        }
        
        return Result.Continue<IConcreteTypeBuilder>();
    }
}
