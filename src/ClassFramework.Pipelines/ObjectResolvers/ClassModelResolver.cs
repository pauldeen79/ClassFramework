namespace ClassFramework.Pipelines.ObjectResolvers;

public class ClassModelResolver : IObjectResolverProcessor
{
    public Result<T> Resolve<T>(object? sourceObject)
        => typeof(T) == typeof(ClassModel)
            ? sourceObject switch
            {
                PipelineContext<BuilderContext> builderContext => Result.Success((T)(object)new ClassModel(builderContext.Request.SourceModel)),
                PipelineContext<BuilderExtensionContext> builderExtensionContext => Result.Success((T)(object)new ClassModel(builderExtensionContext.Request.SourceModel)),
                PipelineContext<EntityContext> entityContext => Result.Success((T)(object)new ClassModel(entityContext.Request.SourceModel)),
                PipelineContext<InterfaceContext> interfaceContext => Result.Success((T)(object)new ClassModel(interfaceContext.Request.SourceModel)),
                PipelineContext<Reflection.ReflectionContext> reflectionContext => Result.Success((T)(object)new ClassModel(reflectionContext.Request.SourceModel)),
                ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder => Result.Success((T)(object)new ClassModel(parentChildContextBuilder.ParentContext.Request.SourceModel)),
                ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension => Result.Success((T)(object)new ClassModel(parentChildContextBuilderExtension.ParentContext.Request.SourceModel)),
                ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => Result.Success((T)(object)new ClassModel(parentChildContextEntity.ParentContext.Request.SourceModel)),
                _ => Result.NotSupported<T>($"Could not get class from context, because the context type {sourceObject?.GetType().FullName ?? "null"} is not supported")
            }
            : Result.Continue<T>();
}
