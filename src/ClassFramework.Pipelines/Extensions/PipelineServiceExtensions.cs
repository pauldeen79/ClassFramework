namespace ClassFramework.Pipelines.Extensions;

public static class PipelineServiceExtensions
{
    public static Task<Result<TypeBase>> Process(this IPipelineService instance, BuilderExtensionContext context)
        => instance.Process(context, CancellationToken.None);

    public static Task<Result<TypeBase>> Process(this IPipelineService instance, BuilderContext context)
        => instance.Process(context, CancellationToken.None);

    public static Task<Result<TypeBase>> Process(this IPipelineService instance, EntityContext context)
            => instance.Process(context, CancellationToken.None);

    public static Task<Result<Domain.Types.Interface>> Process(this IPipelineService instance, InterfaceContext context)
        => instance.Process(context, CancellationToken.None);

    public static Task<Result<TypeBase>> Process(this IPipelineService instance, Reflection.ReflectionContext context)
        => instance.Process(context, CancellationToken.None);
}
