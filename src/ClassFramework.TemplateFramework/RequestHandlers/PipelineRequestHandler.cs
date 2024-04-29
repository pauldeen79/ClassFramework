namespace ClassFramework.TemplateFramework.RequestHandlers;

public class ClassFrameworkPipelineRequestHandler<TResponse, TContext> : IRequestHandler<PipelineRequest<TContext, TResponse>, Result<TResponse>>
    where TContext : ContextBase<TypeBase, TResponse>
{
    private readonly IPipeline<TContext> _pipeline;

    public ClassFrameworkPipelineRequestHandler(IPipeline<TContext> pipeline)
    {
        _pipeline = pipeline;
    }

    public async Task<Result<TResponse>> Handle(PipelineRequest<TContext, TResponse> request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request);

        var result = await _pipeline.Process(request.Context, cancellationToken);
        return Result.FromExistingResult(result, request.Context.ResponseBuilder.Build());
    }
}

public class ReflectionPipelineRequestHandler<TModel, TContext> : IRequestHandler<PipelineRequest<TContext, TModel>, Result<TModel>>
    where TContext : ContextBase<Type, TModel>
{
    private readonly IPipeline<TContext> _pipeline;

    public ReflectionPipelineRequestHandler(IPipeline<TContext> pipeline)
    {
        _pipeline = pipeline;
    }

    public async Task<Result<TModel>> Handle(PipelineRequest<TContext, TModel> request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request);

        var result = await _pipeline.Process(request.Context, cancellationToken);
        if (!result.IsSuccessful())
        {
            return Result.FromExistingResult<TModel>(result);
        }

        //TODO: Validate the ResponseBuilder. When it's invalid, then return validation errors. (else we would throw an exception, but we're returning a Result so we can do this better)

        return Result.FromExistingResult(result, request.Context.ResponseBuilder.Build());
    }
}
