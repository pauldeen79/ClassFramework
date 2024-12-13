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
            "property.InitializationExpression" => GetValueFromProperty(context, (settings, _, property, typeName) => GetInitializationExpression(property, typeName, settings)),

            _ => Result.Continue<object?>()
        };

    private static Result<object?> GetValueFromProperty(object? context, Func<PipelineSettings, CultureInfo, Property, string, object?> valueDelegate)
        => context switch
        {
            PropertyContext propertyContext => Result.Success(valueDelegate(propertyContext.Settings, propertyContext.FormatProvider.ToCultureInfo(), propertyContext.SourceModel, propertyContext.TypeName)),
            ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContextBuilder => Result.Success(valueDelegate(parentChildContextBuilder.Settings, parentChildContextBuilder.ParentContext.Request.FormatProvider.ToCultureInfo(), parentChildContextBuilder.ChildContext, parentChildContextBuilder.ParentContext.Request.MapTypeName(parentChildContextBuilder.ChildContext.TypeName))),
            ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContextBuilderExtension => Result.Success(valueDelegate(parentChildContextBuilderExtension.Settings, parentChildContextBuilderExtension.ParentContext.Request.FormatProvider.ToCultureInfo(), parentChildContextBuilderExtension.ChildContext, parentChildContextBuilderExtension.ParentContext.Request.MapTypeName(parentChildContextBuilderExtension.ChildContext.TypeName))),
            ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => Result.Success(valueDelegate(parentChildContextEntity.Settings, parentChildContextEntity.ParentContext.Request.FormatProvider.ToCultureInfo(), parentChildContextEntity.ChildContext, parentChildContextEntity.ParentContext.Request.MapTypeName(parentChildContextEntity.ChildContext.TypeName))),
            _ => Result.Invalid<object?>($"Could not get property from context, because the context type {context?.GetType().FullName ?? "null"} is not supported")
        };

    private static string GetNullableRequiredSuffix(PipelineSettings settings, Property property)
        => !settings.AddNullChecks && !property.IsValueType && !property.IsNullable && settings.EnableNullableReferenceTypes
            ? "!"
            : string.Empty;

    private static string GetInitializationExpression(Property property, string typeName, PipelineSettings settings)
    {
        return typeName.FixTypeName().IsCollectionTypeName()
            && (settings.CollectionTypeName.Length == 0 || settings.CollectionTypeName != property.TypeName.WithoutProcessedGenerics())
                ? GetCollectionFormatStringForInitialization(property, typeName, settings)
                : "{CsharpFriendlyName(ToCamelCase($property.Name))}{$property.NullableRequiredSuffix}";
    }

    private static string GetCollectionFormatStringForInitialization(Property property, string typeName, PipelineSettings settings)
    {
        var collectionTypeName = settings.CollectionTypeName.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics());

        var genericTypeName = typeName.GetProcessedGenericArguments();

        return property.IsNullable || (settings.AddNullChecks && settings.ValidateArguments != ArgumentValidationType.None)
            ? $"{{ToCamelCase($property.Name)}} {{NullCheck()}} ? null{{$property.NullableRequiredSuffix}} : new {collectionTypeName}<{genericTypeName}>({{CsharpFriendlyName(ToCamelCase($property.Name))}}{{$property.NullableRequiredSuffix}})"
            : $"new {collectionTypeName}<{genericTypeName}>({{CsharpFriendlyName(ToCamelCase($property.Name))}}{{$property.NullableRequiredSuffix}})";
    }
}
