namespace ClassFramework.Pipelines.Abstractions;

public interface IPipelineService
{
    Task<Result<TypeBase>> Process(BuilderExtensionContext context, CancellationToken cancellationToken);
    Task<Result<TypeBase>> Process(BuilderContext context, CancellationToken cancellationToken);
    Task<Result<TypeBase>> Process(EntityContext context, CancellationToken cancellationToken);
    Task<Result<Domain.Types.Interface>> Process(InterfaceContext context, CancellationToken cancellationToken);
    Task<Result<TypeBase>> Process(Reflection.ReflectionContext context, CancellationToken cancellationToken);
}
