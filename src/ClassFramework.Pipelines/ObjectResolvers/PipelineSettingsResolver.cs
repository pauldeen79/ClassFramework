namespace ClassFramework.Pipelines.ObjectResolvers;

public class PipelineSettingsResolver : IObjectResolver
{
    public Result<T> Resolve<T>(object? sourceObject)
        => typeof(T) == typeof(PipelineSettings)
            ? sourceObject switch
            {
                PropertyContext propertyContext => Result.Success((T)(object)propertyContext.Settings),
                ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder => Result.Success((T)(object)parentChildContextBuilder.Settings),
                ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension => Result.Success((T)(object)parentChildContextBuilderExtension.Settings),
                ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => Result.Success((T)(object)parentChildContextEntity.Settings),
                _ => Result.NotSupported<T>($"Could not get pipeline settings from context, because the context type {sourceObject?.GetType().FullName ?? "null"} is not supported")
            }
            : Result.Continue<T>();
}
