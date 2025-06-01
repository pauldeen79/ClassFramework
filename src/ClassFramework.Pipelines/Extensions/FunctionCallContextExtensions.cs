namespace ClassFramework.Pipelines.Extensions;

public static class FunctionCallContextExtensions
{
    public static Task<Result<PipelineSettings>> GetSettingsAsync(this FunctionCallContext instance)
        => instance.Context.State.TryCastValueAsync<PipelineSettings>(ResultNames.Settings);

    public static Task<Result<string>> GetTypeNameAsync(this FunctionCallContext instance)
        => instance.Context.State.TryCastValueAsync<string>(ResultNames.TypeName);

    public static Task<Result<MappedContextBase>> GetMappedContextBaseAsync(this FunctionCallContext instance)
        => instance.Context.State.TryCastValueAsync<MappedContextBase>(ResultNames.Context);
}
