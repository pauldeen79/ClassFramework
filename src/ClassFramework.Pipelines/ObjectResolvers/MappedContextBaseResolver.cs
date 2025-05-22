//namespace ClassFramework.Pipelines.ObjectResolvers;

//public class MappedContextBaseResolver : IObjectResolverProcessor
//{
//    public Result<T> Resolve<T>(object? sourceObject)
//        => typeof(T) == typeof(MappedContextBase)
//            ? sourceObject switch
//            {
//                MappedContextBase mappedContextBase => Result.Success((T)(object)mappedContextBase),
//                ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder => Result.Success((T)(object)parentChildContextBuilder.ParentContext.Request),
//                ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension => Result.Success((T)(object)parentChildContextBuilderExtension.ParentContext.Request),
//                ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => Result.Success((T)(object)parentChildContextEntity.ParentContext.Request),
//                _ => Result.NotSupported<T>($"Could not get {nameof(MappedContextBase)} from context, because the context type {sourceObject?.GetType().FullName ?? "null"} is not supported")
//            }
//            : Result.Continue<T>();
//}
