namespace ClassFramework.Pipelines.Variables;

internal static class VariableBase
{
    internal static Result<object?> GetValueFromProperty(object? context, Func<PipelineSettings, CultureInfo, Property, string, object?> valueDelegate)
        => context switch
        {
            PropertyContext propertyContext => Result.Success(valueDelegate(propertyContext.Settings, propertyContext.FormatProvider.ToCultureInfo(), propertyContext.SourceModel, propertyContext.TypeName)),
            ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder => Result.Success(valueDelegate(parentChildContextBuilder.Settings, parentChildContextBuilder.ParentContext.Request.FormatProvider.ToCultureInfo(), parentChildContextBuilder.ChildContext, parentChildContextBuilder.ParentContext.Request.MapTypeName(parentChildContextBuilder.ChildContext.TypeName))),
            ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension => Result.Success(valueDelegate(parentChildContextBuilderExtension.Settings, parentChildContextBuilderExtension.ParentContext.Request.FormatProvider.ToCultureInfo(), parentChildContextBuilderExtension.ChildContext, parentChildContextBuilderExtension.ParentContext.Request.MapTypeName(parentChildContextBuilderExtension.ChildContext.TypeName))),
            ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => Result.Success(valueDelegate(parentChildContextEntity.Settings, parentChildContextEntity.ParentContext.Request.FormatProvider.ToCultureInfo(), parentChildContextEntity.ChildContext, parentChildContextEntity.ParentContext.Request.MapTypeName(parentChildContextEntity.ChildContext.TypeName))),
            _ => Result.Invalid<object?>($"Could not get property from context, because the context type {context?.GetType().FullName ?? "null"} is not supported")
        };
}
