﻿namespace ClassFramework.Pipelines.Shared.PlaceholderProcessors;

public class PropertyProcessor(ICsharpExpressionDumper csharpExpressionDumper) : IPipelinePlaceholderProcessor, IPlaceholderProcessor
{
    private readonly ICsharpExpressionDumper _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));

    public int Order => 30;

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
            nameof(Property.TypeName) => Result.Success<FormattableStringParserResult>(typeName),
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
            "BuilderAddMethodName" => formattableStringParser.Parse(propertyContext.Settings.AddMethodNameFormatString, formatProvider, propertyContext),
            _ => Result.Continue<FormattableStringParserResult>()
        };
    }

    private static string WithoutInterfacePrefix(string className)
        => className.StartsWith("I")
        && className.Length >= 2
        && className.Substring(1, 1).Equals(className.Substring(1, 1).ToUpperInvariant(), StringComparison.Ordinal)
            ? className.Substring(1)
            : className;
}
