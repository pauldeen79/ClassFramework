namespace ClassFramework.Pipelines.Shared.Variables;

public class PropertyVariable : IVariable
{
    public Result<object?> Process(string variableExpression, object? context)
        => variableExpression switch
        {
            $"property.{nameof(Property.Name)}" => GetValueFromProperty(context, x => x.Name),
            _ => Result.Continue<object?>()
        };

    private static Result<object?> GetValueFromProperty(object? context, Func<Property, object?> valueDelegate)
        => context switch
        {
            PropertyContext propertyContext => Result.Success(valueDelegate(propertyContext.SourceModel)),
            ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder => Result.Success(valueDelegate(parentChildContextBuilder.ChildContext)),
            ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension => Result.Success(valueDelegate(parentChildContextBuilderExtension.ChildContext)),
            _ => Result.Invalid<object?>($"Could not get property from context, because the context type {context?.GetType().FullName} is not supported")
        };
}
