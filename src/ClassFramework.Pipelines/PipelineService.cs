namespace ClassFramework.Pipelines;

public class PipelineService(
    IPipeline<BuilderContext> builderPipeline,
    IPipeline<BuilderExtensionContext> builderExtensionPipeline,
    IPipeline<EntityContext> entityPipeline,
    IPipeline<InterfaceContext> interfacePipeline,
    IPipeline<Reflection.ReflectionContext> reflectionPipeline) : IPipelineService
{
    public async Task<Result<TypeBase>> Process(BuilderExtensionContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        return (await builderExtensionPipeline.Process(context, cancellationToken).ConfigureAwait(false))
            .ProcessResult(context.Builder, context.Builder.Build);
    }

    public async Task<Result<TypeBase>> Process(BuilderContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        return (await builderPipeline.Process(context, cancellationToken).ConfigureAwait(false))
            .ProcessResult(context.Builder, context.Builder.Build);
    }

    public async Task<Result<TypeBase>> Process(EntityContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        return (await entityPipeline.Process(context, cancellationToken).ConfigureAwait(false))
            .ProcessResult(context.Builder, context.Builder.Build);
    }

    public async Task<Result<Domain.Types.Interface>> Process(InterfaceContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        return (await interfacePipeline.Process(context, cancellationToken).ConfigureAwait(false))
            .ProcessResult(context.Builder, context.Builder.BuildTyped);
    }

    public async Task<Result<TypeBase>> Process(Reflection.ReflectionContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        return (await reflectionPipeline.Process(context, cancellationToken).ConfigureAwait(false))
            .ProcessResult(context.Builder, context.Builder.Build);
    }
}
