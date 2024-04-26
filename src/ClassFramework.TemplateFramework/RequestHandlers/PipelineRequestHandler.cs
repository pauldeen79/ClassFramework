﻿namespace ClassFramework.TemplateFramework.RequestHandlers;

public class PipelineRequestHandler<TModel, TContext, TResponse> : IRequestHandler<PipelineRequest<TContext, TResponse>, Result<TResponse>>
    where TModel : TResponse, new()
{
    private readonly IPipeline<TContext, TResponse> _pipeline;

    public PipelineRequestHandler(IPipeline<TContext, TResponse> pipeline)
    {
        _pipeline = pipeline;
    }

    public async Task<Result<TResponse>> Handle(PipelineRequest<TContext, TResponse> request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request);

        var model = new TModel();
        var result = await _pipeline.Process(request.Context, model, cancellationToken);
        return Result.FromExistingResult(result, (TResponse)model);
    }
}
