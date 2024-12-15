namespace ClassFramework.Pipelines.ObjectResolvers;

public class CultureInfoResolver : IObjectResolver
{
    public Result<T> Resolve<T>(object? sourceObject)
        => typeof(T) == typeof(CultureInfo)
            ? sourceObject switch
            {
                PropertyContext propertyContext => Result.Success((T)(object)propertyContext.FormatProvider.ToCultureInfo()),
                ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder => Result.Success((T)(object)parentChildContextBuilder.ParentContext.Request.FormatProvider.ToCultureInfo()),
                ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension => Result.Success((T)(object)parentChildContextBuilderExtension.ParentContext.Request.FormatProvider.ToCultureInfo()),
                ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => Result.Success((T)(object)parentChildContextEntity.ParentContext.Request.FormatProvider.ToCultureInfo()),
                _ => Result.NotSupported<T>($"Could not get culture info from context, because the context type {sourceObject?.GetType().FullName ?? "null"} is not supported")
            }
            : Result.Continue<T>();
}
