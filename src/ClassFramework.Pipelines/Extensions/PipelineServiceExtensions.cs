namespace ClassFramework.Pipelines.Extensions;

public static class PipelineServiceExtensions
{
    public static Task<Result<TypeBase>> ProcessAsync(this IPipelineService instance, BuilderExtensionContext context)
        => instance.ProcessAsync(context, CancellationToken.None);

    public static Task<Result<TypeBase>> ProcessAsync(this IPipelineService instance, BuilderContext context)
        => instance.ProcessAsync(context, CancellationToken.None);

    public static Task<Result<TypeBase>> ProcessAsync(this IPipelineService instance, EntityContext context)
            => instance.ProcessAsync(context, CancellationToken.None);

    public static Task<Result<Domain.Types.Interface>> ProcessAsync(this IPipelineService instance, InterfaceContext context)
        => instance.ProcessAsync(context, CancellationToken.None);

    public static Task<Result<TypeBase>> ProcessAsync(this IPipelineService instance, Reflection.ReflectionContext context)
        => instance.ProcessAsync(context, CancellationToken.None);
}
