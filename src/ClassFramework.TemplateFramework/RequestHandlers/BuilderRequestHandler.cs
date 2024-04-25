namespace ClassFramework.TemplateFramework.RequestHandlers;

public class BuilderRequestHandler : IRequestHandler<BuilderRequest, Result>
{
    private readonly IPipeline<IConcreteTypeBuilder, BuilderContext> _pipeline;

    public BuilderRequestHandler(IPipeline<IConcreteTypeBuilder, BuilderContext> pipeline)
    {
        _pipeline = pipeline;
    }

    public Task<Result> Handle(BuilderRequest request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request);

        return _pipeline.Process(request.Model, request.Context, cancellationToken);
    }
}
