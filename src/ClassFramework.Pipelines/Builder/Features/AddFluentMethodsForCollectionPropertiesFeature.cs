namespace ClassFramework.Pipelines.Builder.Features;

public class AddFluentMethodsForCollectionPropertiesFeatureBuilder : IBuilderFeatureBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddFluentMethodsForCollectionPropertiesFeatureBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineFeature<IConcreteTypeBuilder, BuilderContext> Build()
        => new AddFluentMethodsForCollectionPropertiesFeature(_formattableStringParser);
}

public class AddFluentMethodsForCollectionPropertiesFeature : IPipelineFeature<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddFluentMethodsForCollectionPropertiesFeature(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Context.Settings.AddMethodNameFormatString))
        {
            return Result.Continue<IConcreteTypeBuilder>();
        }

        foreach (var property in context.Context.GetSourceProperties().Where(x => context.Context.IsValidForFluentMethod(x) && x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = CreateParentChildContext(context, property);

            var results = context.Context.GetResultsForBuilderCollectionProperties(property, parentChildContext, _formattableStringParser, GetCodeStatementsForEnumerableOverload(context, property), GetCodeStatementsForArrayOverload(context, property));

            var error = Array.Find(results, x => !x.Result.IsSuccessful());
            if (error is not null)
            {
                // Error in formattable string parsing
                return Result.FromExistingResult<IConcreteTypeBuilder>(error.Result);
            }

            var returnType = context.Context.IsBuilderForAbstractEntity
                ? $"TBuilder{context.Context.SourceModel.GetGenericTypeArgumentsString()}"
                : $"{results.First(x => x.Name == "Namespace").Result.Value.AppendWhenNotNullOrEmpty(".")}{results.First(x => x.Name == "BuilderName").Result.Value}{context.Context.SourceModel.GetGenericTypeArgumentsString()}";

            context.Model.AddMethods(new MethodBuilder()
                .WithName(results.First(x => x.Name == "AddMethodName").Result.Value!)
                .WithReturnTypeName(returnType)
                .AddParameters
                (
                    new ParameterBuilder()
                        .WithName(property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()))
                        .WithTypeName(results.First(x => x.Name == "TypeName").Result.Value!.FixCollectionTypeName(typeof(IEnumerable<>).WithoutGenerics()))
                        .WithIsNullable(property.IsNullable)
                        .WithIsValueType(property.IsValueType)
                )
                .AddStringCodeStatements(results.Where(x => x.Name == "EnumerableOverload").Select(x => x.Result.Value!))
            );

            context.Model.AddMethods(new MethodBuilder()
                .WithName(results.First(x => x.Name == "AddMethodName").Result.Value!)
                .WithReturnTypeName(returnType)
                .AddParameters
                (
                    new ParameterBuilder()
                        .WithName(property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()))
                        .WithTypeName(results.First(x => x.Name == "TypeName").Result.Value!.FixTypeName().ConvertTypeNameToArray())
                        .WithIsParamArray()
                        .WithIsNullable(property.IsNullable)
                        .WithIsValueType(property.IsValueType)
                )
                .AddStringCodeStatements(results.Where(x => x.Name == "ArrayOverload").Select(x => x.Result.Value!))
            );
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, BuilderContext>> ToBuilder()
        => new AddFluentMethodsForCollectionPropertiesFeatureBuilder(_formattableStringParser);

    private IEnumerable<Result<string>> GetCodeStatementsForEnumerableOverload(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, Property property)
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
        yield return Result.Success(context.Context.Settings.AddNullChecks
            ? $"return Add{property.Name}({property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()}?.ToArray() ?? throw new {typeof(ArgumentNullException).FullName}(nameof({property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()})));"
            : $"return Add{property.Name}({property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()}.ToArray());");
    }

    private IEnumerable<Result<string>> GetCodeStatementsForArrayOverload(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, Property property)
    {
        if (context.Context.Settings.AddNullChecks)
        {
            var argumentNullCheckResult = _formattableStringParser.Parse
            (
                context.Context.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{NullCheck.Argument}"),
                context.Context.FormatProvider,
                new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, property, context.Context.Settings)
            );

            if (!string.IsNullOrEmpty(argumentNullCheckResult.Value) || !argumentNullCheckResult.IsSuccessful())
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

        yield return Result.Success($"return {GetReturnValue(context.Context)};");
    }

    private static string GetReturnValue(BuilderContext context)
    {
        if (context.IsBuilderForAbstractEntity)
        {
            return "(TBuilder)this";
        }

        return "this";
    }

    private static ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property> CreateParentChildContext(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, Property property)
        => new(context, property, context.Context.Settings);
}
