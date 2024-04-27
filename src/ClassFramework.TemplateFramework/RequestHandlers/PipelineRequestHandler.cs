namespace ClassFramework.TemplateFramework.RequestHandlers;

public class PipelineRequestHandler<TModel, TContext> : IRequestHandler<PipelineRequest<TContext, TModel>, Result<TModel>>
    where TContext : ContextBase
{
    private readonly IPipeline<TContext, TModel> _pipeline;

    public PipelineRequestHandler(IPipeline<TContext, TModel> pipeline)
    {
        _pipeline = pipeline;
    }

    public async Task<Result<TModel>> Handle(PipelineRequest<TContext, TModel> request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request);

        var model = (TModel)request.Context.CreateModel();
        var result = await _pipeline.Process(request.Context, model, cancellationToken);
        return Result.FromExistingResult(result, model);
    }
}
