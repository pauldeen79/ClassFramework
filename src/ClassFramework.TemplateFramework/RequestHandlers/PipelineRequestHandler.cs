namespace ClassFramework.TemplateFramework.RequestHandlers;

public abstract class PipelineRequestHandler<TResponse, TContext> : IRequestHandler<PipelineRequest<TContext, TResponse>, Result<TResponse>>
{
    private readonly IPipeline<TContext> _pipeline;

    protected PipelineRequestHandler(IPipeline<TContext> pipeline)
    {
        Guard.IsNotNull(pipeline);

        _pipeline = pipeline;
    }

    public async Task<Result<TResponse>> Handle(PipelineRequest<TContext, TResponse> request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request);

        var result = await _pipeline.Process(request.Context, cancellationToken);

        if (!result.IsSuccessful())
        {
            return Result.FromExistingResult<TResponse>(result);
        }

        var validationResults = new List<ValidationResult>();
        var responseBuilder = GetResponseBuilder(request.Context);
        var success = responseBuilder.TryValidate(validationResults);
        if (!success)
        {
            return Result.Invalid<TResponse>("Pipeline response is not valid", validationResults.Select(x => new ValidationError(x.ErrorMessage ?? string.Empty, x.MemberNames)));
        }

        return Result.FromExistingResult(result, BuildResponseValue(request.Context));
    }

    protected abstract IBuilder<TResponse> GetResponseBuilder(TContext context);
    protected abstract TResponse BuildResponseValue(TContext context);
}

public class ClassFrameworkPipelineRequestHandler<TResponse, TContext> : PipelineRequestHandler<TResponse, TContext>
    where TContext : ContextBase<TypeBase, TResponse>
{
    public ClassFrameworkPipelineRequestHandler(IPipeline<TContext> pipeline) : base(pipeline)
    {
    }

    protected override TResponse BuildResponseValue(TContext context) => context.IsNotNull(nameof(context)).ResponseBuilder.Build();

    protected override IBuilder<TResponse> GetResponseBuilder(TContext context) => context.IsNotNull(nameof(context)).ResponseBuilder;
}

public class ReflectionPipelineRequestHandler<TResponse, TContext> : PipelineRequestHandler<TResponse, TContext>
    where TContext : ContextBase<Type, TResponse>
{
    public ReflectionPipelineRequestHandler(IPipeline<TContext> pipeline) : base(pipeline)
    {
    }

    protected override TResponse BuildResponseValue(TContext context) => context.IsNotNull(nameof(context)).ResponseBuilder.Build();

    protected override IBuilder<TResponse> GetResponseBuilder(TContext context) => context.IsNotNull(nameof(context)).ResponseBuilder;
}
