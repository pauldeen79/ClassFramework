namespace ClassFramework.Pipelines.Variables;

internal static class VariableBase
{
    internal static Result<object?> GetValueFromProperty(IObjectResolver objectResolver, object? context, Func<PipelineSettings, CultureInfo, Property, string, object?> valueDelegate)
    {
        var propertyResult = objectResolver.Resolve<Property>(context);
        if (!propertyResult.IsSuccessful())
        {
            return Result.FromExistingResult<object?>(propertyResult);
        }

        //TODO: Use object resolver for PipelineSettings and CultureInfo as well, then simply use a single return statement.

        return context switch
        {
            PropertyContext propertyContext => Result.Success(valueDelegate(propertyContext.Settings, propertyContext.FormatProvider.ToCultureInfo(), propertyResult.Value!, propertyContext.TypeName)),
            ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder => Result.Success(valueDelegate(parentChildContextBuilder.Settings, parentChildContextBuilder.ParentContext.Request.FormatProvider.ToCultureInfo(), propertyResult.Value!, parentChildContextBuilder.ParentContext.Request.MapTypeName(parentChildContextBuilder.ChildContext.TypeName))),
            ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension => Result.Success(valueDelegate(parentChildContextBuilderExtension.Settings, parentChildContextBuilderExtension.ParentContext.Request.FormatProvider.ToCultureInfo(), propertyResult.Value!, parentChildContextBuilderExtension.ParentContext.Request.MapTypeName(parentChildContextBuilderExtension.ChildContext.TypeName))),
            ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => Result.Success(valueDelegate(parentChildContextEntity.Settings, parentChildContextEntity.ParentContext.Request.FormatProvider.ToCultureInfo(), propertyResult.Value!, parentChildContextEntity.ParentContext.Request.MapTypeName(parentChildContextEntity.ChildContext.TypeName))),
            _ => Result.Invalid<object?>($"Could not get property from context, because the context type {context?.GetType().FullName ?? "null"} is not supported")
        };
    }
}
