namespace ClassFramework.Pipelines;

public class PipelineService : IPipelineService
{
    private readonly IPipeline<BuilderContext> _builderPipeline;
    private readonly IPipeline<BuilderExtensionContext> _builderExtensionPipeline;
    private readonly IPipeline<EntityContext> _entityPipeline;
    private readonly IPipeline<InterfaceContext> _interfacePipeline;
    private readonly IPipeline<Reflection.ReflectionContext> _reflectionPipeline;

    public PipelineService(
        IPipeline<BuilderContext> builderPipeline,
        IPipeline<BuilderExtensionContext> builderExtensionPipeline,
        IPipeline<EntityContext> entityPipeline,
        IPipeline<InterfaceContext> interfacePipeline,
        IPipeline<Reflection.ReflectionContext> reflectionPipeline)
    {
        _builderPipeline = builderPipeline;
        _builderExtensionPipeline = builderExtensionPipeline;
        _entityPipeline = entityPipeline;
        _interfacePipeline = interfacePipeline;
        _reflectionPipeline = reflectionPipeline;
    }

    public async Task<Result<TypeBase>> Process(BuilderExtensionContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        var result = await _builderExtensionPipeline.Process(context, cancellationToken).ConfigureAwait(false);
        return ProcessResult(result, context.Builder, context.Builder.Build);
    }

    public async Task<Result<TypeBase>> Process(BuilderContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        var result = await _builderPipeline.Process(context, cancellationToken).ConfigureAwait(false);
        return ProcessResult(result, context.Builder, context.Builder.Build);
    }

    public async Task<Result<TypeBase>> Process(EntityContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        var result = await _entityPipeline.Process(context, cancellationToken).ConfigureAwait(false);
        return ProcessResult(result, context.Builder, context.Builder.Build);
    }

    public async Task<Result<Domain.Types.Interface>> Process(InterfaceContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        var result = await _interfacePipeline.Process(context, cancellationToken).ConfigureAwait(false);
        return ProcessResult(result, context.Builder, context.Builder.BuildTyped);
    }

    public async Task<Result<TypeBase>> Process(Reflection.ReflectionContext context, CancellationToken cancellationToken)
    {
        context = context.IsNotNull(nameof(context));
        var result = await _reflectionPipeline.Process(context, cancellationToken).ConfigureAwait(false);
        return ProcessResult(result, context.Builder, context.Builder.Build);
    }

    private static Result<TResult> ProcessResult<TResult>(Result result, object responseBuilder, Func<TResult> dlg)
    {
        if (!result.IsSuccessful())
        {
            return Result.FromExistingResult<TResult>(result);
        }

        var validationResults = new List<ValidationResult>();
        var success = responseBuilder.TryValidate(validationResults);
        if (!success)
        {
            return Result.Invalid<TResult>("Pipeline response is not valid", validationResults.Select(x => new ValidationError(x.ErrorMessage ?? string.Empty, x.MemberNames)));
        }

        return Result.FromExistingResult(result, dlg.Invoke());
    }
}
