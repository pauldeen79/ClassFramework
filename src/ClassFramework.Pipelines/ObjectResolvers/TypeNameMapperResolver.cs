namespace ClassFramework.Pipelines.ObjectResolvers;

public class TypeNameMapperResolver : IObjectResolver
{
    public Result<T> Resolve<T>(object? sourceObject)
        => typeof(T) == typeof(ITypeNameMapper)
            ? sourceObject switch
            {
                PropertyContext propertyContext => Result.Success((T)(object)new TypeNameMapperWrapper(propertyContext)),
                ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder => Result.Success((T)(object)parentChildContextBuilder.ParentContext.Request),
                ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension => Result.Success((T)(object)parentChildContextBuilderExtension.ParentContext.Request),
                ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => Result.Success((T)(object)parentChildContextEntity.ParentContext.Request),
                _ => Result.NotSupported<T>($"Could not get typename mappe from context, because the context type {sourceObject?.GetType().FullName ?? "null"} is not supported")
            }
            : Result.Continue<T>();

    private sealed class TypeNameMapperWrapper : ITypeNameMapper
    {
        private readonly PropertyContext _propertyContext;

        public TypeNameMapperWrapper(PropertyContext propertyContext) => _propertyContext = propertyContext;

        public string MapTypeName(string typeName, string alternateTypeMetadataName) => _propertyContext.TypeName;
    }
}
