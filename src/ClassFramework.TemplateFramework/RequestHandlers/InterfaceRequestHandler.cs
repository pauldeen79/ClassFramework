namespace ClassFramework.TemplateFramework.RequestHandlers;

public class InterfaceRequestHandler : IRequestHandler<InterfaceRequest, Result>
{
    private readonly IPipeline<InterfaceBuilder, InterfaceContext> _pipeline;

    public InterfaceRequestHandler(IPipeline<InterfaceBuilder, InterfaceContext> pipeline)
    {
        _pipeline = pipeline;
    }

    public Task<Result> Handle(InterfaceRequest request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request);

        return _pipeline.Process(request.Model, request.Context, cancellationToken);
    }
}
