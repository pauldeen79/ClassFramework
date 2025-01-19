namespace ClassFramework.Pipelines.Abstractions;

public interface IPipelineService
{
    Task<Result<TypeBase>> ProcessAsync(BuilderExtensionContext context, CancellationToken cancellationToken);
    Task<Result<TypeBase>> ProcessAsync(BuilderContext context, CancellationToken cancellationToken);
    Task<Result<TypeBase>> ProcessAsync(EntityContext context, CancellationToken cancellationToken);
    Task<Result<Domain.Types.Interface>> ProcessAsync(InterfaceContext context, CancellationToken cancellationToken);
    Task<Result<TypeBase>> ProcessAsync(Reflection.ReflectionContext context, CancellationToken cancellationToken);
}
