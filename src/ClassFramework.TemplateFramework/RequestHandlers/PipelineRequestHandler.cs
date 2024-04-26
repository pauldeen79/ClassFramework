namespace ClassFramework.TemplateFramework.RequestHandlers;

public class PipelineRequestHandler<TModel, TContext> : IRequestHandler<PipelineRequest<TModel, TContext>, Result>
{
    private readonly IPipeline<TModel, TContext> _pipeline;

    public PipelineRequestHandler(IPipeline<TModel, TContext> pipeline)
    {
        _pipeline = pipeline;
    }

    public Task<Result> Handle(PipelineRequest<TModel, TContext> request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request);

        return _pipeline.Process(request.Model, request.Context, cancellationToken);
    }
}
