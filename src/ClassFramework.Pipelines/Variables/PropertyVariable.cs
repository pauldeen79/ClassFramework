namespace ClassFramework.Pipelines.Variables;

public class PropertyVariable : IVariable
{
    public Result<object?> Process(string variableExpression, object? context)
        => variableExpression switch
        {
            $"property.{nameof(Property.Name)}" => VariableBase.GetValueFromProperty(context, (_, _, property, _) => property.Name),
            $"property.{nameof(Property.TypeName)}" => VariableBase.GetValueFromProperty(context, (_, _, _, typeName) => typeName),
            "property.BuilderMemberName" => VariableBase.GetValueFromProperty(context, (settings, culture, property, _) => property.GetBuilderMemberName(settings, culture)),
            "property.EntityMemberName" => VariableBase.GetValueFromProperty(context, (settings, culture, property, _) => property.GetEntityMemberName(settings.AddBackingFields || settings.CreateAsObservable, culture)),
            "property.NullableRequiredSuffix" => VariableBase.GetValueFromProperty(context, (settings, _, property, _) => GetNullableRequiredSuffix(settings, property)),
            "property.InitializationExpression" => VariableBase.GetValueFromProperty(context, (settings, _, property, typeName) => GetInitializationExpression(property, typeName, settings)),
            _ => Result.Continue<object?>()
        };

    private static string GetNullableRequiredSuffix(PipelineSettings settings, Property property)
        => !settings.AddNullChecks && !property.IsValueType && !property.IsNullable && settings.EnableNullableReferenceTypes
            ? "!"
            : string.Empty;

    private static string GetInitializationExpression(Property property, string typeName, PipelineSettings settings)
        => typeName.FixTypeName().IsCollectionTypeName()
            && (settings.CollectionTypeName.Length == 0 || settings.CollectionTypeName != property.TypeName.WithoutProcessedGenerics())
                ? GetCollectionFormatStringForInitialization(property, settings)
                : "{CsharpFriendlyName(ToCamelCase($property.Name))}{$property.NullableRequiredSuffix}";

    private static string GetCollectionFormatStringForInitialization(Property property, PipelineSettings settings)
        => property.IsNullable || (settings.AddNullChecks && settings.ValidateArguments != ArgumentValidationType.None)
            ? "{ToCamelCase($property.Name)} {NullCheck()} ? null{$property.NullableRequiredSuffix} : new {$collectionTypeName}<{GenericArguments($property.TypeName)}>({CsharpFriendlyName(ToCamelCase($property.Name))}{$property.NullableRequiredSuffix})"
            : "new {$collectionTypeName}<{GenericArguments($property.TypeName)}>({CsharpFriendlyName(ToCamelCase($property.Name))}{$property.NullableRequiredSuffix})";
}
