﻿namespace ClassFramework.Pipelines.Builder.Components;

public class AddFluentMethodsForCollectionPropertiesComponentBuilder(IFormattableStringParser formattableStringParser) : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public IPipelineComponent<BuilderContext> Build()
        => new AddFluentMethodsForCollectionPropertiesComponent(_formattableStringParser);
}

public class AddFluentMethodsForCollectionPropertiesComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> Process(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Request.Settings.AddMethodNameFormatString))
        {
            return Task.FromResult(Result.Continue());
        }

        foreach (var property in context.Request.GetSourceProperties().Where(x => context.Request.IsValidForFluentMethod(x) && x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = CreateParentChildContext(context, property);

            var results = context.Request.GetResultsForBuilderCollectionProperties(property, parentChildContext, _formattableStringParser, GetCodeStatementsForEnumerableOverload(context, property, parentChildContext), GetCodeStatementsForArrayOverload(context, property));

            var error = Array.Find(results, x => !x.Result.IsSuccessful());
            if (error is not null)
            {
                // Error in formattable string parsing
                return Task.FromResult<Result>(error.Result);
            }

            var returnType = context.Request.IsBuilderForAbstractEntity
                ? $"TBuilder{context.Request.SourceModel.GetGenericTypeArgumentsString()}"
                : $"{results.First(x => x.Name == "Namespace").Result.Value!.ToString().AppendWhenNotNullOrEmpty(".")}{results.First(x => x.Name == "BuilderName").Result.Value}{context.Request.SourceModel.GetGenericTypeArgumentsString()}";

            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithName(results.First(x => x.Name == "AddMethodName").Result.Value!)
                .WithReturnTypeName(returnType)
                .AddParameters(context.Request.CreateParameterForBuilder(property, results.First(x => x.Name == "TypeName").Result.Value!.ToString().FixCollectionTypeName(typeof(IEnumerable<>).WithoutGenerics())))
                .AddStringCodeStatements(results.Where(x => x.Name == "EnumerableOverload").Select(x => x.Result.Value!.ToString()))
            );

            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithName(results.First(x => x.Name == "AddMethodName").Result.Value!)
                .WithReturnTypeName(returnType)
                .AddParameters(context.Request.CreateParameterForBuilder(property, results.First(x => x.Name == "TypeName").Result.Value!.ToString().FixTypeName().ConvertTypeNameToArray()).WithIsParamArray())
                .AddStringCodeStatements(results.Where(x => x.Name == "ArrayOverload").Select(x => x.Result.Value!.ToString()))
            );
        }

        return Task.FromResult(Result.Continue());
    }

    private IEnumerable<Result<FormattableStringParserResult>> GetCodeStatementsForEnumerableOverload(PipelineContext<BuilderContext> context, Property property, ParentChildContext<PipelineContext<BuilderContext>, Property> parentChildContext)
    {
        if (context.Request.Settings.BuilderNewCollectionTypeName == typeof(IEnumerable<>).WithoutGenerics())
        {
            // When using IEnumerable<>, do not call ToArray because we want lazy evaluation
            foreach (var statement in GetCodeStatementsForArrayOverload(context, property))
            {
                yield return statement;
            }
            yield break;
        }

        // When not using IEnumerable<>, we can simply force ToArray because it's stored in a generic list or collection of some sort anyway.
        // (in other words, materialization is always performed)
        if (context.Request.Settings.AddNullChecks)
        {
            yield return Result.Success<FormattableStringParserResult>(context.Request.CreateArgumentNullException(property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()));
        }

        yield return _formattableStringParser.Parse("return {$addMethodNameFormatString}({CsharpFriendlyName(ToCamelCase($property.Name))}.ToArray());", context.Request.FormatProvider, parentChildContext);
    }

    private IEnumerable<Result<FormattableStringParserResult>> GetCodeStatementsForArrayOverload(PipelineContext<BuilderContext> context, Property property)
    {
        if (context.Request.Settings.AddNullChecks)
        {
            var argumentNullCheckResult = _formattableStringParser.Parse
            (
                context.Request.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{NullCheck.Argument}"),
                context.Request.FormatProvider,
                new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings)
            );

            if (!argumentNullCheckResult.IsSuccessful() || !string.IsNullOrEmpty(argumentNullCheckResult.Value!.ToString()))
            {
                yield return argumentNullCheckResult;
            }
        }

        var builderAddExpressionResult = _formattableStringParser.Parse
        (
            context.Request.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderAddExpression, context.Request.Settings.CollectionCopyStatementFormatString),
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings)
        );

        yield return builderAddExpressionResult;

        yield return Result.Success<FormattableStringParserResult>(context.Request.ReturnValueStatementForFluentMethod);
    }

    private static ParentChildContext<PipelineContext<BuilderContext>, Property> CreateParentChildContext(PipelineContext<BuilderContext> context, Property property)
        => new(context, property, context.Request.Settings);
}
