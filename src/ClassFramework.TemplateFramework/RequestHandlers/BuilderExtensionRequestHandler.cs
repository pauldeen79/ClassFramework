namespace ClassFramework.TemplateFramework.RequestHandlers;

public class BuilderExtensionRequestHandler : IRequestHandler<BuilderExtensionRequest, Result>
{
    private readonly IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> _pipeline;

    public BuilderExtensionRequestHandler(IPipeline<IConcreteTypeBuilder, BuilderExtensionContext> pipeline)
    {
        _pipeline = pipeline;
    }

    public Task<Result> Handle(BuilderExtensionRequest request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request);

        return _pipeline.Process(request.Model, request.Context, cancellationToken);
    }
}
