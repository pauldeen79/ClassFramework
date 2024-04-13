﻿namespace ClassFramework.Pipelines.Builder.Features;

public class AddFluentMethodsForCollectionPropertiesComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddFluentMethodsForCollectionPropertiesComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build()
        => new AddFluentMethodsForCollectionPropertiesComponent(_formattableStringParser);
}

public class AddFluentMethodsForCollectionPropertiesComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddFluentMethodsForCollectionPropertiesComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Context.Settings.AddMethodNameFormatString))
        {
            return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
        }

        foreach (var property in context.Context.GetSourceProperties().Where(x => context.Context.IsValidForFluentMethod(x) && x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = CreateParentChildContext(context, property);

            var results = context.Context.GetResultsForBuilderCollectionProperties(property, parentChildContext, _formattableStringParser, GetCodeStatementsForEnumerableOverload(context, property, parentChildContext), GetCodeStatementsForArrayOverload(context, property));

            var error = Array.Find(results, x => !x.Result.IsSuccessful());
            if (error is not null)
            {
                // Error in formattable string parsing
                return Task.FromResult(Result.FromExistingResult<IConcreteTypeBuilder>(error.Result));
            }

            var returnType = context.Context.IsBuilderForAbstractEntity
                ? $"TBuilder{context.Context.SourceModel.GetGenericTypeArgumentsString()}"
                : $"{results.First(x => x.Name == "Namespace").Result.Value!.ToString().AppendWhenNotNullOrEmpty(".")}{results.First(x => x.Name == "BuilderName").Result.Value}{context.Context.SourceModel.GetGenericTypeArgumentsString()}";

            context.Model.AddMethods(new MethodBuilder()
                .WithName(results.First(x => x.Name == "AddMethodName").Result.Value!)
                .WithReturnTypeName(returnType)
                .AddParameters(context.Context.CreateParameterForBuilder(property, results.First(x => x.Name == "TypeName").Result.Value!.ToString().FixCollectionTypeName(typeof(IEnumerable<>).WithoutGenerics())))
                .AddStringCodeStatements(results.Where(x => x.Name == "EnumerableOverload").Select(x => x.Result.Value!.ToString()))
            );

            context.Model.AddMethods(new MethodBuilder()
                .WithName(results.First(x => x.Name == "AddMethodName").Result.Value!)
                .WithReturnTypeName(returnType)
                .AddParameters(context.Context.CreateParameterForBuilder(property, results.First(x => x.Name == "TypeName").Result.Value!.ToString().FixTypeName().ConvertTypeNameToArray()).WithIsParamArray())
                .AddStringCodeStatements(results.Where(x => x.Name == "ArrayOverload").Select(x => x.Result.Value!.ToString()))
            );
        }

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }

    private IEnumerable<Result<FormattableStringParserResult>> GetCodeStatementsForEnumerableOverload(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, Property property, ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property> parentChildContext)
    {
        if (context.Context.Settings.BuilderNewCollectionTypeName == typeof(IEnumerable<>).WithoutGenerics())
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
        if (context.Context.Settings.AddNullChecks)
        {
            yield return Result.Success(context.Context.CreateArgumentNullException(property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()).ToFormattableStringParserResult());
        }
        
        yield return _formattableStringParser.Parse("return {BuilderAddMethodName}({NamePascalCsharpFriendlyName}.ToArray());", context.Context.FormatProvider, parentChildContext);
    }

    private IEnumerable<Result<FormattableStringParserResult>> GetCodeStatementsForArrayOverload(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, Property property)
    {
        if (context.Context.Settings.AddNullChecks)
        {
            var argumentNullCheckResult = _formattableStringParser.Parse
            (
                context.Context.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{NullCheck.Argument}"),
                context.Context.FormatProvider,
                new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, property, context.Context.Settings)
            );

            if (!argumentNullCheckResult.IsSuccessful() || !string.IsNullOrEmpty(argumentNullCheckResult.Value!.ToString()))
            {
                yield return argumentNullCheckResult;
            }
        }

        var builderAddExpressionResult = _formattableStringParser.Parse
        (
            context.Context.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderAddExpression, context.Context.Settings.CollectionCopyStatementFormatString),
            context.Context.FormatProvider,
            new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, property, context.Context.Settings)
        );

        yield return builderAddExpressionResult;

        yield return Result.Success(context.Context.ReturnValueStatementForFluentMethod.ToFormattableStringParserResult());
    }

    private static ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property> CreateParentChildContext(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, Property property)
        => new(context, property, context.Context.Settings);
}
