namespace ClassFramework.TemplateFramework.RequestHandlers;

public class ClassFrameworkPipelineRequestHandler<TResponse, TContext> : IRequestHandler<PipelineRequest<TContext, TResponse>, Result<TResponse>>
    where TContext : ContextBase<TypeBase, TResponse>
{
    private readonly IPipeline<TContext> _pipeline;

    public ClassFrameworkPipelineRequestHandler(IPipeline<TContext> pipeline)
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
        var success = request.Context.ResponseBuilder.TryValidate(validationResults);
        if (!success)
        {
            return Result.Invalid<TResponse>("Pipeline response is not valid", validationResults.Select(x => new ValidationError(x.ErrorMessage ?? string.Empty, x.MemberNames)));
        }

        return Result.FromExistingResult(result, request.Context.ResponseBuilder.Build());
    }
}

public class ReflectionPipelineRequestHandler<TResponse, TContext> : IRequestHandler<PipelineRequest<TContext, TResponse>, Result<TResponse>>
    where TContext : ContextBase<Type, TResponse>
{
    private readonly IPipeline<TContext> _pipeline;

    public ReflectionPipelineRequestHandler(IPipeline<TContext> pipeline)
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
        var success = request.Context.ResponseBuilder.TryValidate(validationResults);
        if (!success)
        {
            return Result.Invalid<TResponse>("Pipeline response is not valid", validationResults.Select(x => new ValidationError(x.ErrorMessage ?? string.Empty, x.MemberNames)));
        }

        return Result.FromExistingResult(result, request.Context.ResponseBuilder.Build());
    }
}
