namespace ClassFramework.Pipelines.Shared.PlaceholderProcessors;

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
            $"{nameof(Property.TypeName)}.CollectionItemType.GenericArgumentsWithBrackets" => Result.Success<FormattableStringParserResult>(typeName.GetCollectionItemType().GetProcessedGenericArguments(addBrackets: true)),
            $"{nameof(Property.TypeName)}.CollectionItemType.GenericArgumentsWithoutBrackets" => Result.Success<FormattableStringParserResult>(typeName.GetCollectionItemType().GetProcessedGenericArguments(addBrackets: false)),
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
}
