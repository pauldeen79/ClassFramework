namespace ClassFramework.Pipelines.Shared.PlaceholderProcessors;

public class PropertyProcessor : IPipelinePlaceholderProcessor, IPlaceholderProcessor
{
    private readonly ICsharpExpressionDumper _csharpExpressionDumper;

    public int Order => 30;

    public PropertyProcessor(ICsharpExpressionDumper csharpExpressionDumper)
    {
        _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));
    }

    public Result<FormattableStringParserResult> Process(string value, IFormatProvider formatProvider, object? context, IFormattableStringParser formattableStringParser)
    {
        formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

        if (context is not PropertyContext propertyContext)
        {
            return Result.Continue<FormattableStringParserResult>();
        }

        var typeName = propertyContext.TypeName.FixTypeName();

        return value switch
        {
            nameof(Property.Name) => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.Name),
            $"{nameof(Property.Name)}Lower" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.Name.ToLower(formatProvider.ToCultureInfo())),
            $"{nameof(Property.Name)}Upper" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.Name.ToUpper(formatProvider.ToCultureInfo())),
            $"{nameof(Property.Name)}Pascal" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.Name.ToPascalCase(formatProvider.ToCultureInfo())),
            $"{nameof(Property.Name)}PascalCsharpFriendlyName" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.Name.ToPascalCase(formatProvider.ToCultureInfo()).GetCsharpFriendlyName()),
            $"{nameof(Property.Name)}Camel" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.Name.ToCamelCase(formatProvider.ToCultureInfo())),
            $"{nameof(Property.Name)}CamelCsharpFriendlyName" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.Name.ToCamelCase(formatProvider.ToCultureInfo()).GetCsharpFriendlyName()),
            "BuilderMemberName" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.GetBuilderMemberName(propertyContext.Settings, propertyContext.FormatProvider.ToCultureInfo())),
            "EntityMemberName" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.GetEntityMemberName(propertyContext.Settings.AddBackingFields || propertyContext.Settings.CreateAsObservable, propertyContext.FormatProvider.ToCultureInfo())),
            "InitializationExpression" => Result.Success<FormattableStringParserResult>(GetInitializationExpression(propertyContext.SourceModel, typeName, propertyContext.Settings.CollectionTypeName, formatProvider.ToCultureInfo(), propertyContext.Settings, propertyContext.NullCheck)),
            "CollectionTypeName" => Result.Success<FormattableStringParserResult>(propertyContext.Settings.CollectionTypeName),
            nameof(Property.TypeName) => Result.Success<FormattableStringParserResult>(typeName),
            $"{nameof(Property.TypeName)}.GenericArguments" => Result.Success<FormattableStringParserResult>(typeName.GetProcessedGenericArguments()),
            $"{nameof(Property.TypeName)}.GenericArgumentsWithBrackets" => Result.Success<FormattableStringParserResult>(typeName.GetProcessedGenericArguments(addBrackets: true)),
            $"{nameof(Property.TypeName)}.GenericArgumentsWithoutBrackets" => Result.Success<FormattableStringParserResult>(typeName.GetProcessedGenericArguments(addBrackets: false)),
            $"{nameof(Property.TypeName)}.GenericArguments.ClassName" => Result.Success<FormattableStringParserResult>(typeName.GetProcessedGenericArguments().GetClassName()),
            $"{nameof(Property.TypeName)}.GenericArguments.ClassName.NoGenerics" => Result.Success<FormattableStringParserResult>(typeName.GetProcessedGenericArguments().GetClassName().WithoutProcessedGenerics()),
            $"{nameof(Property.TypeName)}.CollectionItemType.GenericArgumentsWithBrackets" => Result.Success<FormattableStringParserResult>(typeName.GetCollectionItemType().GetProcessedGenericArguments(addBrackets: true)),
            $"{nameof(Property.TypeName)}.CollectionItemType.GenericArgumentsWithoutBrackets" => Result.Success<FormattableStringParserResult>(typeName.GetCollectionItemType().GetProcessedGenericArguments(addBrackets: false)),
            $"{nameof(Property.TypeName)}.ClassName" => Result.Success<FormattableStringParserResult>(typeName.GetClassName()),
            $"{nameof(Property.TypeName)}.ClassName.NoGenerics" => Result.Success<FormattableStringParserResult>(typeName.GetClassName().WithoutProcessedGenerics()),
            $"{nameof(Property.TypeName)}.ClassName.NoInterfacePrefix" => Result.Success<FormattableStringParserResult>(WithoutInterfacePrefix(typeName.GetClassName())),
            $"{nameof(Property.TypeName)}.Namespace" => Result.Success<FormattableStringParserResult>(typeName.GetNamespaceWithDefault()),
            $"{nameof(Property.TypeName)}.NoGenerics" => Result.Success<FormattableStringParserResult>(typeName.WithoutProcessedGenerics()),
            "ParentTypeName" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.ParentTypeFullName),
            "ParentTypeName.ClassName" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.ParentTypeFullName.GetClassName()),
            "ParentTypeName.ClassName.NoGenerics" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.ParentTypeFullName.GetClassName().WithoutProcessedGenerics()),
            "ParentTypeName.GenericArgumentsWithBrackets" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.ParentTypeFullName.GetClassName().GetProcessedGenericArguments(addBrackets: true)),
            "ParentTypeName.GenericArgumentsWithoutBrackets" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.ParentTypeFullName.GetClassName().GetProcessedGenericArguments(addBrackets: false)),
            "DefaultValue" => formattableStringParser.Parse(propertyContext.SourceModel.GetDefaultValue(_csharpExpressionDumper, typeName, propertyContext), formatProvider, propertyContext),
            "NullableSuffix" => Result.Success<FormattableStringParserResult>(propertyContext.SourceModel.GetSuffix(propertyContext.Settings.EnableNullableReferenceTypes, _csharpExpressionDumper, propertyContext)),
            "BuilderAddMethodName" => formattableStringParser.Parse(propertyContext.Settings.AddMethodNameFormatString, formatProvider, propertyContext),
            _ => Result.Continue<FormattableStringParserResult>()
        };
    }

    private static string GetInitializationExpression(Property property, string typeName, string collectionTypeName, CultureInfo cultureInfo, PipelineSettings settings, string nullCheck)
    {
        collectionTypeName = collectionTypeName.IsNotNull(nameof(collectionTypeName));

        return typeName.FixTypeName().IsCollectionTypeName()
            && (collectionTypeName.Length == 0 || collectionTypeName != property.TypeName.WithoutProcessedGenerics())
                ? GetCollectionFormatStringForInitialization(property, typeName, cultureInfo, collectionTypeName, settings, nullCheck)
                : property.Name.ToCamelCase(cultureInfo).GetCsharpFriendlyName();
    }

    private static string GetCollectionFormatStringForInitialization(Property property, string typeName, CultureInfo cultureInfo, string collectionTypeName, PipelineSettings settings, string nullCheck)
    {
        collectionTypeName = collectionTypeName.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics());

        var genericTypeName = typeName.GetProcessedGenericArguments();
        var nullSuffix = settings.EnableNullableReferenceTypes && !property.IsNullable
            ? "!"
            : string.Empty;

        return property.IsNullable || (settings.AddNullChecks && settings.ValidateArguments != ArgumentValidationType.None)
            ? $"{property.Name.ToCamelCase(cultureInfo)} {nullCheck} ? null{nullSuffix} : new {collectionTypeName}<{genericTypeName}>({property.Name.ToCamelCase(cultureInfo).GetCsharpFriendlyName()})"
            : $"new {collectionTypeName}<{genericTypeName}>({property.Name.ToCamelCase(cultureInfo).GetCsharpFriendlyName()})";
    }

    private static string WithoutInterfacePrefix(string className)
        => className.StartsWith("I")
        && className.Length >= 2
        && className.Substring(1, 1).Equals(className.Substring(1, 1).ToUpperInvariant(), StringComparison.Ordinal)
            ? className.Substring(1)
            : className;
}
