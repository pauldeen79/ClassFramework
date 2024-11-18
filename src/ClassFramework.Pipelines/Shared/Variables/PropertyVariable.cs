namespace ClassFramework.Pipelines.Shared.Variables;

public class PropertyVariable : IVariable
{
    public Result<object?> Process(string variableExpression, object? context)
    {
        return variableExpression switch
        {
            $"property.{nameof(Property.Name)}" => GetValueFromProperty(context, x => x.Name),
            _ => Result.Continue<object?>()
        };
    }

    private static Result<object?> GetValueFromProperty(object? context, Func<Property, object?> valueDelegate)
    {
        if (context is PropertyContext propertyContext)
        {
            return Result.Success<object?>(valueDelegate(propertyContext.SourceModel));
        }

        if (context is ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder)
        {
            return Result.Success<object?>(valueDelegate(parentChildContextBuilder.ChildContext));
        }

        if (context is ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension)
        {
            return Result.Success<object?>(valueDelegate(parentChildContextBuilderExtension.ChildContext));
        }

        return Result.Invalid<object?>($"Could not get property from context, because the context type {context?.GetType().FullName} is not supported");
    }
}
