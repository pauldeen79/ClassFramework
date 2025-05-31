namespace ClassFramework.Pipelines.Extensions;

public static class FunctionCallContextExtensions
{
    public static Task<Result<PipelineSettings>> GetSettingsAsync(this FunctionCallContext instance)
        => instance.Context.State.TryCastValueAsync<PipelineSettings>("settings");

    public static Task<Result<string>> GetTypeNameAsync(this FunctionCallContext instance)
        => instance.Context.State.TryCastValueAsync<string>("typename");

    public static Task<Result<MappedContextBase>> GetMappedContextBaseAsync(this FunctionCallContext instance)
        => instance.Context.State.TryCastValueAsync<MappedContextBase>("context");
}
