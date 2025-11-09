namespace ClassFramework.Pipelines.Extensions;

public static class FunctionCallContextExtensions
{
    public static Task<Result<PipelineSettings>> GetSettingsAsync(this FunctionCallContext instance)
        => instance.Context.State.TryCastValueAsync<PipelineSettings>(ResultNames.Settings);

    public static Task<Result<string>> GetTypeNameAsync(this FunctionCallContext instance)
        => instance.Context.State.TryCastValueAsync<string>(ResultNames.TypeName);

    public static Task<Result<MappedCommandBase>> GetMappedContextBaseAsync(this FunctionCallContext instance)
        => instance.Context.State.TryCastValueAsync<MappedCommandBase>(ResultNames.Context);

    public static async Task<Result<object?>> EvaluateForProperty(this FunctionCallContext instance, Func<Property, PipelineSettings, string> evaluationDelegate)
        => (await new AsyncResultDictionaryBuilder()
            .Add(Constants.Instance, instance.GetInstanceValueResult<Property>())
            .Add(ResultNames.Settings, instance.GetSettingsAsync)
            .BuildAsync()
            .ConfigureAwait(false))
            .OnSuccess<object?>(results => evaluationDelegate(
                results.GetValue<Property>(Constants.Instance),
                results.GetValue<PipelineSettings>(ResultNames.Settings)));

    public static async Task<Result<object?>> EvaluateForProperty(this FunctionCallContext instance, Func<Property, PipelineSettings, MappedCommandBase, string> evaluationDelegate)
        => (await new AsyncResultDictionaryBuilder()
            .Add(Constants.Instance, instance.GetInstanceValueResult<Property>())
            .Add(ResultNames.Settings, instance.GetSettingsAsync)
            .Add(ResultNames.Context, instance.GetMappedContextBaseAsync)
            .BuildAsync()
            .ConfigureAwait(false))
            .OnSuccess<object?>(results => evaluationDelegate(
                results.GetValue<Property>(Constants.Instance),
                results.GetValue<PipelineSettings>(ResultNames.Settings),
                results.GetValue<MappedCommandBase>(ResultNames.Context)));
}
