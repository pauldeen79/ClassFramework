namespace ClassFramework.TemplateFramework.RequestHandlers;

public class ReflectionRequestHandler : IRequestHandler<ReflectionRequest, Result>
{
    private readonly IPipeline<TypeBaseBuilder, ReflectionContext> _pipeline;

    public ReflectionRequestHandler(IPipeline<TypeBaseBuilder, ReflectionContext> pipeline)
    {
        _pipeline = pipeline;
    }

    public Task<Result> Handle(ReflectionRequest request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request);

        return _pipeline.Process(request.Model, request.Context, cancellationToken);
    }
}
