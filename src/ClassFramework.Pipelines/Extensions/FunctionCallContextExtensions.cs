namespace ClassFramework.Pipelines.Extensions;

public static class FunctionCallContextExtensions
{
    public static async Task<Result<PipelineSettings>> GetSettingsAsync(this FunctionCallContext instance)
        => instance.Context.State.TryGetValue("settings", out var settingsResult)
            ? (await settingsResult.ConfigureAwait(false))
                .TryCastAllowNull<PipelineSettings>()
                .EnsureValue()
            : Result.Invalid<PipelineSettings>("settings was not found in state");

    public static async Task<Result<string>> GetTypeNameAsync(this FunctionCallContext instance)
        => instance.Context.State.TryGetValue("typename", out var typeNameResult)
            ? (await typeNameResult.ConfigureAwait(false))
                .TryCastAllowNull<string>()
                .EnsureValue()
            : Result.Invalid<string>("typename was not found in state");

    public static async Task<Result<MappedContextBase>> GetMappedContextBaseAsync(this FunctionCallContext instance)
        => instance.Context.State.TryGetValue("context", out var typeNameResult)
            ? (await typeNameResult.ConfigureAwait(false))
                .TryCastAllowNull<MappedContextBase>()
                .EnsureValue()
            : Result.Invalid<MappedContextBase>("context was not found in state");
}
