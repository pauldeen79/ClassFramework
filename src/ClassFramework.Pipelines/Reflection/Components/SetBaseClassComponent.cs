namespace ClassFramework.Pipelines.Reflection.Components;

public class SetBaseClassComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext, TypeBaseBuilder> Build()
        => new SetBaseClassComponent();
}

public class SetBaseClassComponent : IPipelineComponent<ReflectionContext, TypeBaseBuilder>
{
    public Task<Result> Process(PipelineContext<ReflectionContext, TypeBaseBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Response is IBaseClassContainerBuilder baseClassContainerBuilder)
        {
            baseClassContainerBuilder.WithBaseClass(context.Request.SourceModel.GetEntityBaseClass(context.Request.Settings));
        }

        return Task.FromResult(Result.Continue());
    }
}
