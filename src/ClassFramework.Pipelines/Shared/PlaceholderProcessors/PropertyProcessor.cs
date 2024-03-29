namespace ClassFramework.Pipelines.Shared.PlaceholderProcessors;

public class PropertyProcessor : IPipelinePlaceholderProcessor
{
    private readonly ICsharpExpressionDumper _csharpExpressionDumper;

    public PropertyProcessor(ICsharpExpressionDumper csharpExpressionDumper)
    {
        _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));
    }

    public Result<string> Process(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser)
    {
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        if (context is not PropertyContext propertyContext)
        {
            return Result.Continue<string>();
        }

        var typeName = propertyContext.TypeName.FixTypeName();

        return value switch
        {
            nameof(Property.Name) => Result.Success(propertyContext.SourceModel.Name),
            $"{nameof(Property.Name)}Lower" => Result.Success(propertyContext.SourceModel.Name.ToLower(formatProvider.ToCultureInfo())),
            $"{nameof(Property.Name)}Upper" => Result.Success(propertyContext.SourceModel.Name.ToUpper(formatProvider.ToCultureInfo())),
            $"{nameof(Property.Name)}Pascal" => Result.Success(propertyContext.SourceModel.Name.ToPascalCase(formatProvider.ToCultureInfo())),
            $"{nameof(Property.Name)}PascalCsharpFriendlyName" => Result.Success(propertyContext.SourceModel.Name.ToPascalCase(formatProvider.ToCultureInfo()).GetCsharpFriendlyName()),
            "BuilderMemberName" => Result.Success(propertyContext.SourceModel.GetBuilderMemberName(propertyContext.Settings, propertyContext.FormatProvider.ToCultureInfo())),
            "EntityMemberName" => Result.Success(propertyContext.SourceModel.GetEntityMemberName(propertyContext.Settings.AddBackingFields || propertyContext.Settings.CreateAsObservable, propertyContext.FormatProvider.ToCultureInfo())),
            "InitializationExpression" => Result.Success(GetInitializationExpression(propertyContext.SourceModel, typeName, propertyContext.Settings.CollectionTypeName, formatProvider.ToCultureInfo(), propertyContext.Settings.AddNullChecks, propertyContext.Settings.ValidateArguments, propertyContext.Settings.EnableNullableReferenceTypes)),
            "CollectionTypeName" => Result.Success(propertyContext.Settings.CollectionTypeName),
            nameof (Property.TypeName) => Result.Success(typeName),
            $"{nameof(Property.TypeName)}.GenericArguments" => Result.Success(typeName.GetProcessedGenericArguments()),
            $"{nameof(Property.TypeName)}.GenericArgumentsWithBrackets" => Result.Success(typeName.GetProcessedGenericArguments(addBrackets: true)),
            $"{nameof(Property.TypeName)}.GenericArgumentsWithoutBrackets" => Result.Success(typeName.GetProcessedGenericArguments(addBrackets: false)),
            $"{nameof(Property.TypeName)}.GenericArguments.ClassName" => Result.Success(typeName.GetProcessedGenericArguments().GetClassName()),
            $"{nameof(Property.TypeName)}.ClassName" => Result.Success(typeName.GetClassName()),
            $"{nameof(Property.TypeName)}.ClassName.NoGenerics" => Result.Success(typeName.GetClassName().WithoutProcessedGenerics()),
            $"{nameof(Property.TypeName)}.ClassName.NoInterfacePrefix" => Result.Success(WithoutInterfacePrefix(typeName.GetClassName())),
            $"{nameof(Property.TypeName)}.Namespace" => Result.Success(typeName.GetNamespaceWithDefault()),
            $"{nameof(Property.TypeName)}.NoGenerics" => Result.Success(typeName.WithoutProcessedGenerics()),
            "ParentTypeName" => Result.Success(propertyContext.SourceModel.ParentTypeFullName),
            "ParentTypeName.ClassName" => Result.Success(propertyContext.SourceModel.ParentTypeFullName.GetClassName()),
            "ParentTypeName.ClassName.NoGenerics" => Result.Success(propertyContext.SourceModel.ParentTypeFullName.GetClassName().WithoutProcessedGenerics()),
            "ParentTypeName.GenericArgumentsWithBrackets" => Result.Success(propertyContext.SourceModel.ParentTypeFullName.GetClassName().GetProcessedGenericArguments(addBrackets: true)),
            "ParentTypeName.GenericArgumentsWithoutBrackets" => Result.Success(propertyContext.SourceModel.ParentTypeFullName.GetClassName().GetProcessedGenericArguments(addBrackets: false)),
            "DefaultValue" => formattableStringParser.Parse(propertyContext.SourceModel.GetDefaultValue(_csharpExpressionDumper, typeName, propertyContext), formatProvider, context),
            "NullableSuffix" => Result.Success(propertyContext.SourceModel.GetSuffix(propertyContext.Settings.EnableNullableReferenceTypes)),
            _ => Result.Continue<string>()
        };
    }

    private static string GetInitializationExpression(Property property, string typeName, string collectionTypeName, CultureInfo cultureInfo, bool addNullChecks, ArgumentValidationType validateArguments, bool enableNullableReferenceTypes)
    {
        collectionTypeName = collectionTypeName.IsNotNull(nameof(collectionTypeName));

        return typeName.FixTypeName().IsCollectionTypeName()
            && (collectionTypeName.Length == 0 || collectionTypeName != property.TypeName.WithoutGenerics())
                ? GetCollectionFormatStringForInitialization(property, typeName, cultureInfo, collectionTypeName, addNullChecks, validateArguments, enableNullableReferenceTypes)
                : property.Name.ToPascalCase(cultureInfo).GetCsharpFriendlyName();
    }

    private static string GetCollectionFormatStringForInitialization(Property property, string typeName, CultureInfo cultureInfo, string collectionTypeName, bool addNullChecks, ArgumentValidationType validateArguments, bool enableNullableReferenceTypes)
    {
        collectionTypeName = collectionTypeName.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics());

        var genericTypeName = typeName.GetGenericArguments();
        var nullSuffix = enableNullableReferenceTypes && !property.IsNullable
            ? "!"
            : string.Empty;

        return property.IsNullable || (addNullChecks && validateArguments != ArgumentValidationType.None)
            ? $"{property.Name.ToPascalCase(cultureInfo)} is null ? null{nullSuffix} : new {collectionTypeName}<{genericTypeName}>({property.Name.ToPascalCase(cultureInfo).GetCsharpFriendlyName()})"
            : $"new {collectionTypeName}<{genericTypeName}>({property.Name.ToPascalCase(cultureInfo).GetCsharpFriendlyName()})";
    }

    private static string WithoutInterfacePrefix(string className)
        => className.StartsWith("I")
        && className.Length >= 2
        && className.Substring(1, 1).Equals(className.Substring(1, 1).ToUpperInvariant(), StringComparison.Ordinal)
            ? className.Substring(1)
            : className;

}
