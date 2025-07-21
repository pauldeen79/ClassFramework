namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class AddExtensionMethodsForCollectionPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderExtensionContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Request.Settings.AddMethodNameFormatString))
        {
            return Result.Success();
        }

        foreach (var property in context.Request.GetSourceProperties()
            .Where(x => x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = CreateParentChildContext(context, property);

            var results = await context.Request.GetResultDictionaryForBuilderCollectionProperties(property, parentChildContext,_evaluator)
                .AddRange("EnumerableOverload.{0}", await GetCodeStatementsForEnumerableOverload(context, property, parentChildContext, false, token).ConfigureAwait(false))
                .AddRange("ArrayOverload.{0}", await GetCodeStatementsForArrayOverload(context, property, false, token).ConfigureAwait(false))
                .Build()
                .ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            var returnType = context.Request.GetReturnTypeForFluentMethod(results.GetValue(ResultNames.Namespace), results.GetValue(ResultNames.BuilderName));

            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithName(results.GetValue(ResultNames.AddMethodName))
                .WithReturnTypeName("T")
                .WithStatic()
                .WithExtensionMethod()
                .AddGenericTypeArguments("T")
                .AddGenericTypeArgumentConstraints($"where T : {returnType}")
                .AddParameter("instance", "T")
                .AddParameters(context.Request.CreateParameterForBuilder(property, results.GetValue(ResultNames.TypeName).ToString().FixCollectionTypeName(typeof(IEnumerable<>).WithoutGenerics())))
                .AddCodeStatements(results.Where(x => x.Key.StartsWith("EnumerableOverload.")).Select(x => x.Value.Value!.ToString()))
            );

            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithName(results.GetValue(ResultNames.AddMethodName))
                .WithReturnTypeName("T")
                .WithStatic()
                .WithExtensionMethod()
                .AddGenericTypeArguments("T")
                .AddGenericTypeArgumentConstraints($"where T : {returnType}")
                .AddParameter("instance", "T")
                .AddParameters(context.Request.CreateParameterForBuilder(property, results.GetValue(ResultNames.TypeName).ToString().FixTypeName().ConvertTypeNameToArray()).WithIsParamArray())
                .AddCodeStatements(results.Where(x => x.Key.StartsWith("ArrayOverload.")).Select(x => x.Value.Value!.ToString()))
            );

            //TODO: Add functionality to GenericFormattableString, so we can compare them by value (just like string)
            if (results.GetValue(ResultNames.TypeName).ToString() != results.GetValue(ResultNames.NonLazyTypeName).ToString())
            {
                //TODO: Add overloads for non-func type with array and enumerable type
            }
        }

        return Result.Success();
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForEnumerableOverload(PipelineContext<BuilderExtensionContext> context, Property property, ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> parentChildContext, bool useBuilderLazyValues, CancellationToken token)
    {
        var results = new List<Result<GenericFormattableString>>();

        if (context.Request.Settings.AddNullChecks)
        {
            results.Add(Result.Success<GenericFormattableString>(context.Request.CreateArgumentNullException(property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName())));
        }

        results.Add(await _evaluator.EvaluateInterpolatedStringAsync("return instance.{addMethodNameFormatString}<T>({CsharpFriendlyName(property.Name.ToCamelCase())}.ToArray());", context.Request.FormatProvider, parentChildContext, token).ConfigureAwait(false));

        return results;
    }

    private async Task<IEnumerable<Result<GenericFormattableString>>> GetCodeStatementsForArrayOverload(PipelineContext<BuilderExtensionContext> context, Property property, bool useBuilderLazyValues, CancellationToken token)
    {
        var results = new List<Result<GenericFormattableString>>();

        if (context.Request.Settings.AddNullChecks)
        {
            var argumentNullCheckResult = await _evaluator.EvaluateInterpolatedStringAsync
            (
                context.Request.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{ArgumentNullCheck()}"),
                context.Request.FormatProvider,
                new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(context, property, context.Request.Settings),
                token
            ).ConfigureAwait(false);
            results.Add(argumentNullCheckResult);
        }

        var builderAddExpressionResult = await _evaluator.EvaluateInterpolatedStringAsync
        (
            context.Request
                .GetMappingMetadata(property.TypeName)
                .GetStringValue(MetadataNames.CustomBuilderAddExpression, context.Request.Settings.BuilderExtensionsCollectionCopyStatementFormatString),
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(context, property, context.Request.Settings),
            token
        ).ConfigureAwait(false);

        results.Add(builderAddExpressionResult);
        results.Add(Result.Success<GenericFormattableString>("return instance;"));

        return results;
    }

    private static ParentChildContext<PipelineContext<BuilderExtensionContext>, Property> CreateParentChildContext(PipelineContext<BuilderExtensionContext> context, Property property)
        => new(context, property, context.Request.Settings);
}
