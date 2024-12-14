namespace ClassFramework.Pipelines.ObjectResolvers;

public class PropertyResolver : IObjectResolver
{
    public Result<T> Resolve<T>(object? sourceObject)
        => typeof(T) == typeof(Property)
            ? sourceObject switch
            {
                PropertyContext propertyContext => Result.Success((T)(object)propertyContext.SourceModel),
                ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder => Result.Success((T)(object)parentChildContextBuilder.ChildContext),
                ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension => Result.Success((T)(object)parentChildContextBuilderExtension.ChildContext),
                ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => Result.Success((T)(object)parentChildContextEntity.ChildContext),
                _ => Result.NotSupported<T>($"Could not get property from context, because the context type {sourceObject?.GetType().FullName ?? "null"} is not supported")
            }
            : Result.Continue<T>();
}
