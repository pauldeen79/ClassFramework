namespace ClassFramework.Pipelines.Shared.Variables;

public class PropertyVariable : IVariable
{
    public Result<object?> Process(string variableExpression, object? context)
        => variableExpression switch
        {
            $"property.{nameof(Property.Name)}" => GetValueFromProperty(context, (_, _, property, _) => property.Name),
            $"property.{nameof(Property.TypeName)}" => GetValueFromProperty(context, (_, _, _, typeName) => typeName),
            "property.BuilderMemberName" => GetValueFromProperty(context, (settings, culture, property, _) => property.GetBuilderMemberName(settings, culture)),
            "property.EntityMemberName" => GetValueFromProperty(context, (settings, culture, property, _) => property.GetEntityMemberName(settings.AddBackingFields || settings.CreateAsObservable, culture)),
            "property.NullableRequiredSuffix" => GetValueFromProperty(context, (settings, _, property, _) => GetNullableRequiredSuffix(settings, property)),
            _ => Result.Continue<object?>()
        };

    private static Result<object?> GetValueFromProperty(object? context, Func<PipelineSettings, CultureInfo, Property, string, object?> valueDelegate)
        => context switch
        {
            PropertyContext propertyContext => Result.Success(valueDelegate(propertyContext.Settings, propertyContext.FormatProvider.ToCultureInfo(), propertyContext.SourceModel, propertyContext.TypeName)),
            ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder => Result.Success(valueDelegate(parentChildContextBuilder.Settings, parentChildContextBuilder.ParentContext.Request.FormatProvider.ToCultureInfo(), parentChildContextBuilder.ChildContext, new PropertyContext(parentChildContextBuilder.ChildContext, parentChildContextBuilder.Settings, parentChildContextBuilder.ParentContext.Request.FormatProvider, parentChildContextBuilder.ParentContext.Request.MapTypeName(parentChildContextBuilder.ChildContext.TypeName), parentChildContextBuilder.Settings.EntityNewCollectionTypeName).TypeName)),
            ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension => Result.Success(valueDelegate(parentChildContextBuilderExtension.Settings, parentChildContextBuilderExtension.ParentContext.Request.FormatProvider.ToCultureInfo(), parentChildContextBuilderExtension.ChildContext, new PropertyContext(parentChildContextBuilderExtension.ChildContext, parentChildContextBuilderExtension.Settings, parentChildContextBuilderExtension.ParentContext.Request.FormatProvider, parentChildContextBuilderExtension.ParentContext.Request.MapTypeName(parentChildContextBuilderExtension.ChildContext.TypeName), parentChildContextBuilderExtension.Settings.EntityNewCollectionTypeName).TypeName)),
            ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => Result.Success(valueDelegate(parentChildContextEntity.Settings, parentChildContextEntity.ParentContext.Request.FormatProvider.ToCultureInfo(), parentChildContextEntity.ChildContext, new PropertyContext(parentChildContextEntity.ChildContext, parentChildContextEntity.Settings, parentChildContextEntity.ParentContext.Request.FormatProvider, parentChildContextEntity.ParentContext.Request.MapTypeName(parentChildContextEntity.ChildContext.TypeName), parentChildContextEntity.Settings.EntityNewCollectionTypeName).TypeName)),
            _ => Result.Invalid<object?>($"Could not get property from context, because the context type {context?.GetType().FullName ?? "null"} is not supported")
        };

    private static string GetNullableRequiredSuffix(PipelineSettings settings, Property property)
        => !settings.AddNullChecks && !property.IsValueType && !property.IsNullable && settings.EnableNullableReferenceTypes
            ? "!"
            : string.Empty;
}
