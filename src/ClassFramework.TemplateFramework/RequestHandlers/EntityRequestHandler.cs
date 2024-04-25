namespace ClassFramework.TemplateFramework.RequestHandlers;

public class EntityRequestHandler : IRequestHandler<EntityRequest, Result>
{
    private readonly IPipeline<IConcreteTypeBuilder, EntityContext> _pipeline;

    public EntityRequestHandler(IPipeline<IConcreteTypeBuilder, EntityContext> pipeline)
    {
        _pipeline = pipeline;
    }

    public Task<Result> Handle(EntityRequest request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request);

        return _pipeline.Process(request.Model, request.Context, cancellationToken);
    }
}
