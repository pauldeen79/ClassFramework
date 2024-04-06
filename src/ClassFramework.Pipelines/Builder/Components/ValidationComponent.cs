namespace ClassFramework.Pipelines.Builder.Features;

public class ValidationComponentBuilder : IBuilderComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build() => new ValidationComponent();
}

public class ValidationComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.AllowGenerationWithoutProperties
            && context.Context.SourceModel.Properties.Count == 0
            && !context.Context.Settings.EnableInheritance)
        {
            return Result.Invalid<IConcreteTypeBuilder>("To create a builder class, there must be at least one property");
        }
        
        return Result.Continue<IConcreteTypeBuilder>();
    }
}
