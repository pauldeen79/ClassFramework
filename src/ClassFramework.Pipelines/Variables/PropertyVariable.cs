﻿namespace ClassFramework.Pipelines.Variables;

public class PropertyVariable : IVariable
{
    private readonly IObjectResolver _objectResolver;
    private readonly ICsharpExpressionDumper _csharpExpressionDumper;

    public PropertyVariable(IObjectResolver objectResolver, ICsharpExpressionDumper csharpExpressionDumper)
    {
        ArgumentGuard.IsNotNull(objectResolver, nameof(objectResolver));
        ArgumentGuard.IsNotNull(csharpExpressionDumper, nameof(csharpExpressionDumper));

        _objectResolver = objectResolver;
        _csharpExpressionDumper = csharpExpressionDumper;
    }

    public Result<object?> Evaluate(string expression, object? context)
        => expression switch
        {
            $"property.{nameof(Property.Name)}" => VariableBase.GetValueFromProperty(_objectResolver, context, (_, _, property, _, _) => property.Name),
            $"property.{nameof(Property.TypeName)}" => VariableBase.GetValueFromProperty(_objectResolver, context, (_, _, _, typeName, _) => typeName),
            $"property.{nameof(Property.ParentTypeFullName)}" => VariableBase.GetValueFromProperty(_objectResolver, context, (_, _, property, typeName, _) => property.ParentTypeFullName),
            "property.BuilderMemberName" => VariableBase.GetValueFromProperty(_objectResolver, context, (settings, culture, property, _, _) => property.GetBuilderMemberName(settings, culture)),
            "property.EntityMemberName" => VariableBase.GetValueFromProperty(_objectResolver, context, (settings, culture, property, _, _) => property.GetEntityMemberName(settings.AddBackingFields || settings.CreateAsObservable, culture)),
            "property.NullableRequiredSuffix" => VariableBase.GetValueFromProperty(_objectResolver, context, (settings, _, property, _, _) => GetNullableRequiredSuffix(settings, property)),
            "property.InitializationExpression" => VariableBase.GetValueFromProperty(_objectResolver, context, (settings, _, property, typeName, _) => GetInitializationExpression(property, typeName, settings)),
            "property.DefaultValue" => VariableBase.GetValueFromProperty(_objectResolver, context, (settings, _, property, typeName, mappedContextBase) => property.GetDefaultValue(_csharpExpressionDumper, typeName, mappedContextBase)),
            _ => Result.Continue<object?>()
        };

    public Result<Type> Validate(string expression, object? context)
        => Result.Success(typeof(object));

    private static string GetNullableRequiredSuffix(PipelineSettings settings, Property property)
        => !settings.AddNullChecks && !property.IsValueType && !property.IsNullable && settings.EnableNullableReferenceTypes
            ? "!"
            : string.Empty;

    private static string GetInitializationExpression(Property property, string typeName, PipelineSettings settings)
        => typeName.FixTypeName().IsCollectionTypeName()
            && (settings.CollectionTypeName.Length == 0 || settings.CollectionTypeName != property.TypeName.WithoutGenerics())
                ? GetCollectionFormatStringForInitialization(property, settings)
                : "{CsharpFriendlyName(ToCamelCase($property.Name))}{$property.NullableRequiredSuffix}";

    private static string GetCollectionFormatStringForInitialization(Property property, PipelineSettings settings)
        => property.IsNullable || (settings.AddNullChecks && settings.ValidateArguments != ArgumentValidationType.None)
            ? $"{{ToCamelCase($property.Name)}} {{NullCheck()}} ? null{GetPropertyInitializationNullSuffix(property, settings)} : new {{$collectionTypeName}}<{{GenericArguments($property.TypeName)}}>({{CsharpFriendlyName(ToCamelCase($property.Name))}})"
            : $"new {{$collectionTypeName}}<{{GenericArguments($property.TypeName)}}>({{CsharpFriendlyName(ToCamelCase($property.Name))}})";

    private static string GetPropertyInitializationNullSuffix(Property property, PipelineSettings settings)
        => settings.EnableNullableReferenceTypes && !property.IsNullable
            ? "!"
            : string.Empty;
}
