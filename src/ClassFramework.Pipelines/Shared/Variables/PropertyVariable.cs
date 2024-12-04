namespace ClassFramework.Pipelines.Shared.Variables;

public class PropertyVariable : IVariable
{
    public Result<object?> Process(string variableExpression, object? context)
        => variableExpression switch
        {
            $"property.{nameof(Property.Name)}" => GetValueFromProperty(context, (_, _, property) => property.Name),
            $"property.builderMemberName" => GetValueFromProperty(context, (settings, culture, property) => property.GetBuilderMemberName(settings, culture)),
            $"property.entityMemberName" => GetValueFromProperty(context, (settings, culture, property) => property.GetEntityMemberName(settings.AddBackingFields || settings.CreateAsObservable, culture)),
            _ => Result.Continue<object?>()
        };

    private static Result<object?> GetValueFromProperty(object? context, Func<PipelineSettings, CultureInfo, Property, object?> valueDelegate)
        => context switch
        {
            PropertyContext propertyContext => Result.Success(valueDelegate(propertyContext.Settings, propertyContext.FormatProvider.ToCultureInfo(), propertyContext.SourceModel)),
            ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder => Result.Success(valueDelegate(parentChildContextBuilder.Settings, parentChildContextBuilder.ParentContext.Request.FormatProvider.ToCultureInfo(), parentChildContextBuilder.ChildContext)),
            ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension => Result.Success(valueDelegate(parentChildContextBuilderExtension.Settings, parentChildContextBuilderExtension.ParentContext.Request.FormatProvider.ToCultureInfo(), parentChildContextBuilderExtension.ChildContext)),
            ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => Result.Success(valueDelegate(parentChildContextEntity.Settings, parentChildContextEntity.ParentContext.Request.FormatProvider.ToCultureInfo(), parentChildContextEntity.ChildContext)),
            _ => Result.Invalid<object?>($"Could not get property from context, because the context type {context?.GetType().FullName ?? "null"} is not supported")
        };
}
