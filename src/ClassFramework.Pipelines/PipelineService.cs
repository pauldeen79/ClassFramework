namespace ClassFramework.Pipelines;

public class PipelineService(
    IPipeline<BuilderContext> builderPipeline,
    IPipeline<BuilderExtensionContext> builderExtensionPipeline,
    IPipeline<EntityContext> entityPipeline,
    IPipeline<InterfaceContext> interfacePipeline,
    IPipeline<Reflection.ReflectionContext> reflectionPipeline) : IPipelineService
{
    public async Task<Result<TypeBase>> ProcessAsync(BuilderExtensionContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        return (await builderExtensionPipeline.ProcessAsync(context, cancellationToken).ConfigureAwait(false))
            .ProcessResult(context.Builder, context.Builder.Build);
    }

    public async Task<Result<TypeBase>> ProcessAsync(BuilderContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        return (await builderPipeline.ProcessAsync(context, cancellationToken).ConfigureAwait(false))
            .ProcessResult(context.Builder, context.Builder.Build);
    }

    public async Task<Result<TypeBase>> ProcessAsync(EntityContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        return (await entityPipeline.ProcessAsync(context, cancellationToken).ConfigureAwait(false))
            .ProcessResult(context.Builder, context.Builder.Build);
    }

    public async Task<Result<Domain.Types.Interface>> ProcessAsync(InterfaceContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        return (await interfacePipeline.ProcessAsync(context, cancellationToken).ConfigureAwait(false))
            .ProcessResult(context.Builder, context.Builder.BuildTyped);
    }

    public async Task<Result<TypeBase>> ProcessAsync(Reflection.ReflectionContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        return (await reflectionPipeline.ProcessAsync(context, cancellationToken).ConfigureAwait(false))
            .ProcessResult(context.Builder, context.Builder.Build);
    }
}
