namespace ClassFramework.Pipelines.Extensions;

public static class FunctionCallContextExtensions
{
    public static Task<Result<PipelineSettings>> GetSettingsAsync(this FunctionCallContext instance)
        => instance.Context.State.TryGetValueAsync<PipelineSettings>("settings");

    public static Task<Result<string>> GetTypeNameAsync(this FunctionCallContext instance)
        => instance.Context.State.TryGetValueAsync<string>("typename");

    public static Task<Result<MappedContextBase>> GetMappedContextBaseAsync(this FunctionCallContext instance)
        => instance.Context.State.TryGetValueAsync<MappedContextBase>("context");
}
