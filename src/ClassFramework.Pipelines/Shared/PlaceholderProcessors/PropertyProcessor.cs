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
            nameof(Property.Name) => Result.Success(propertyContext.SourceModel.Name.ToFormattableStringParserResult()),
            $"{nameof(Property.Name)}Lower" => Result.Success(propertyContext.SourceModel.Name.ToLower(formatProvider.ToCultureInfo()).ToFormattableStringParserResult()),
            $"{nameof(Property.Name)}Upper" => Result.Success(propertyContext.SourceModel.Name.ToUpper(formatProvider.ToCultureInfo()).ToFormattableStringParserResult()),
            $"{nameof(Property.Name)}Pascal" => Result.Success(propertyContext.SourceModel.Name.ToPascalCase(formatProvider.ToCultureInfo()).ToFormattableStringParserResult()),
            $"{nameof(Property.Name)}PascalCsharpFriendlyName" => Result.Success(propertyContext.SourceModel.Name.ToPascalCase(formatProvider.ToCultureInfo()).GetCsharpFriendlyName().ToFormattableStringParserResult()),
            "BuilderMemberName" => Result.Success(propertyContext.SourceModel.GetBuilderMemberName(propertyContext.Settings, propertyContext.FormatProvider.ToCultureInfo()).ToFormattableStringParserResult()),
            "EntityMemberName" => Result.Success(propertyContext.SourceModel.GetEntityMemberName(propertyContext.Settings.AddBackingFields || propertyContext.Settings.CreateAsObservable, propertyContext.FormatProvider.ToCultureInfo()).ToFormattableStringParserResult()),
            "InitializationExpression" => Result.Success(GetInitializationExpression(propertyContext.SourceModel, typeName, propertyContext.Settings.CollectionTypeName, formatProvider.ToCultureInfo(), propertyContext.Settings.AddNullChecks, propertyContext.Settings.ValidateArguments, propertyContext.Settings.EnableNullableReferenceTypes).ToFormattableStringParserResult()),
            "CollectionTypeName" => Result.Success(propertyContext.Settings.CollectionTypeName.ToFormattableStringParserResult()),
            nameof (Property.TypeName) => Result.Success(typeName.ToFormattableStringParserResult()),
            $"{nameof(Property.TypeName)}.GenericArguments" => Result.Success(typeName.GetProcessedGenericArguments().ToFormattableStringParserResult()),
            $"{nameof(Property.TypeName)}.GenericArgumentsWithBrackets" => Result.Success(typeName.GetProcessedGenericArguments(addBrackets: true).ToFormattableStringParserResult()),
            $"{nameof(Property.TypeName)}.GenericArgumentsWithoutBrackets" => Result.Success(typeName.GetProcessedGenericArguments(addBrackets: false).ToFormattableStringParserResult()),
            $"{nameof(Property.TypeName)}.GenericArguments.ClassName" => Result.Success(typeName.GetProcessedGenericArguments().GetClassName().ToFormattableStringParserResult()),
            $"{nameof(Property.TypeName)}.GenericArguments.ClassName.NoGenerics" => Result.Success(typeName.GetProcessedGenericArguments().GetClassName().WithoutProcessedGenerics().ToFormattableStringParserResult()),
            $"{nameof(Property.TypeName)}.CollectionItemType.GenericArgumentsWithBrackets" => Result.Success(typeName.GetCollectionItemType().GetProcessedGenericArguments(addBrackets: true).ToFormattableStringParserResult()),
            $"{nameof(Property.TypeName)}.CollectionItemType.GenericArgumentsWithoutBrackets" => Result.Success(typeName.GetCollectionItemType().GetProcessedGenericArguments(addBrackets: false).ToFormattableStringParserResult()),
            $"{nameof(Property.TypeName)}.ClassName" => Result.Success(typeName.GetClassName().ToFormattableStringParserResult()),
            $"{nameof(Property.TypeName)}.ClassName.NoGenerics" => Result.Success(typeName.GetClassName().WithoutProcessedGenerics().ToFormattableStringParserResult()),
            $"{nameof(Property.TypeName)}.ClassName.NoInterfacePrefix" => Result.Success(WithoutInterfacePrefix(typeName.GetClassName()).ToFormattableStringParserResult()),
            $"{nameof(Property.TypeName)}.Namespace" => Result.Success(typeName.GetNamespaceWithDefault().ToFormattableStringParserResult()),
            $"{nameof(Property.TypeName)}.NoGenerics" => Result.Success(typeName.WithoutProcessedGenerics().ToFormattableStringParserResult()),
            "ParentTypeName" => Result.Success(propertyContext.SourceModel.ParentTypeFullName.ToFormattableStringParserResult()),
            "ParentTypeName.ClassName" => Result.Success(propertyContext.SourceModel.ParentTypeFullName.GetClassName().ToFormattableStringParserResult()),
            "ParentTypeName.ClassName.NoGenerics" => Result.Success(propertyContext.SourceModel.ParentTypeFullName.GetClassName().WithoutProcessedGenerics().ToFormattableStringParserResult()),
            "ParentTypeName.GenericArgumentsWithBrackets" => Result.Success(propertyContext.SourceModel.ParentTypeFullName.GetClassName().GetProcessedGenericArguments(addBrackets: true).ToFormattableStringParserResult()),
            "ParentTypeName.GenericArgumentsWithoutBrackets" => Result.Success(propertyContext.SourceModel.ParentTypeFullName.GetClassName().GetProcessedGenericArguments(addBrackets: false).ToFormattableStringParserResult()),
            "DefaultValue" => formattableStringParser.Parse(propertyContext.SourceModel.GetDefaultValue(_csharpExpressionDumper, typeName, propertyContext), formatProvider, propertyContext),
            "NullableSuffix" => Result.Success(propertyContext.SourceModel.GetSuffix(propertyContext.Settings.EnableNullableReferenceTypes, _csharpExpressionDumper, propertyContext).ToFormattableStringParserResult()),
            "BuilderAddMethodName" => formattableStringParser.Parse(propertyContext.Settings.AddMethodNameFormatString, formatProvider, propertyContext),
            _ => Result.Continue<FormattableStringParserResult>()
        };
    }

    private static string GetInitializationExpression(Property property, string typeName, string collectionTypeName, CultureInfo cultureInfo, bool addNullChecks, ArgumentValidationType validateArguments, bool enableNullableReferenceTypes)
    {
        collectionTypeName = collectionTypeName.IsNotNull(nameof(collectionTypeName));

        return typeName.FixTypeName().IsCollectionTypeName()
            && (collectionTypeName.Length == 0 || collectionTypeName != property.TypeName.WithoutProcessedGenerics())
                ? GetCollectionFormatStringForInitialization(property, typeName, cultureInfo, collectionTypeName, addNullChecks, validateArguments, enableNullableReferenceTypes)
                : property.Name.ToPascalCase(cultureInfo).GetCsharpFriendlyName();
    }

    private static string GetCollectionFormatStringForInitialization(Property property, string typeName, CultureInfo cultureInfo, string collectionTypeName, bool addNullChecks, ArgumentValidationType validateArguments, bool enableNullableReferenceTypes)
    {
        collectionTypeName = collectionTypeName.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics());

        var genericTypeName = typeName.GetProcessedGenericArguments();
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
