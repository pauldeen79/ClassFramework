namespace ClassFramework.Pipelines.Reflection.Features;

public class SetBaseClassComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new SetBaseClassComponent();
}

public class SetBaseClassComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Task<Result<TypeBaseBuilder>> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Model is IBaseClassContainerBuilder baseClassContainerBuilder)
        {
            baseClassContainerBuilder.WithBaseClass(context.Context.SourceModel.GetEntityBaseClass(context.Context.Settings));
        }

        return Task.FromResult(Result.Continue<TypeBaseBuilder>());
    }
}
