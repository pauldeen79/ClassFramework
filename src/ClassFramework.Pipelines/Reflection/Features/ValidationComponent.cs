namespace ClassFramework.Pipelines.Reflection.Features;

public class ValidationComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build() => new ValidationComponent();
}

public class ValidationComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Task<Result<TypeBaseBuilder>> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AllowGenerationWithoutProperties
            && context.Request.SourceModel.GetProperties().Length == 0)
        {
            return Task.FromResult(Result.Invalid<TypeBaseBuilder>("To create a class, there must be at least one property"));
        }
        
        return Task.FromResult(Result.Continue<TypeBaseBuilder>());
    }
}
