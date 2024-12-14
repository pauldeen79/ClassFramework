﻿namespace ClassFramework.Pipelines.Variables;

public class PropertyVariable : IVariable
{
    private readonly IObjectResolver _objectResolver;

    public PropertyVariable(IObjectResolver objectResolver)
    {
        ArgumentGuard.IsNotNull(objectResolver, nameof(objectResolver));

        _objectResolver = objectResolver;
    }

    public Result<object?> Process(string variableExpression, object? context)
        => variableExpression switch
        {
            $"property.{nameof(Property.Name)}" => VariableBase.GetValueFromProperty(_objectResolver, context, (_, _, property, _) => property.Name),
            $"property.{nameof(Property.TypeName)}" => VariableBase.GetValueFromProperty(_objectResolver, context, (_, _, _, typeName) => typeName),
            "property.BuilderMemberName" => VariableBase.GetValueFromProperty(_objectResolver, context, (settings, culture, property, _) => property.GetBuilderMemberName(settings, culture)),
            "property.EntityMemberName" => VariableBase.GetValueFromProperty(_objectResolver, context, (settings, culture, property, _) => property.GetEntityMemberName(settings.AddBackingFields || settings.CreateAsObservable, culture)),
            "property.NullableRequiredSuffix" => VariableBase.GetValueFromProperty(_objectResolver, context, (settings, _, property, _) => GetNullableRequiredSuffix(settings, property)),
            "property.InitializationExpression" => VariableBase.GetValueFromProperty(_objectResolver, context, (settings, _, property, typeName) => GetInitializationExpression(property, typeName, settings)),
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
