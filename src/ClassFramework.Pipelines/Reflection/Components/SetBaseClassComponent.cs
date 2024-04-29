namespace ClassFramework.Pipelines.Reflection.Components;

public class SetBaseClassComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext> Build()
        => new SetBaseClassComponent();
}

public class SetBaseClassComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> Process(PipelineContext<ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.Builder is IBaseClassContainerBuilder baseClassContainerBuilder)
        {
            baseClassContainerBuilder.WithBaseClass(context.Request.SourceModel.GetEntityBaseClass(context.Request.Settings));
        }

        return Task.FromResult(Result.Continue());
    }
}
