namespace ClassFramework.Pipelines.Reflection.Features;

public class ValidationComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build() => new ValidationComponent();
}

public class ValidationComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.AllowGenerationWithoutProperties
            && context.Context.SourceModel.GetProperties().Length == 0)
        {
            return Result.Invalid<TypeBaseBuilder>("To create a class, there must be at least one property");
        }
        
        return Result.Continue<TypeBaseBuilder>();
    }
}
