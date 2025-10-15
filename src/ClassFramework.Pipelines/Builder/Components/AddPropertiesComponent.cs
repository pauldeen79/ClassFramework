namespace ClassFramework.Pipelines.Builder.Components;

public class AddPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>, IOrderContainer
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public int Order => PipelineStage.Process;

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.IsAbstractBuilder)
        {
            return Result.Continue();
        }

        foreach (var property in context.Request.GetSourceProperties())
        {
            var results = await new AsyncResultDictionaryBuilder<GenericFormattableString>()
                .Add(ResultNames.TypeName, property.GetBuilderArgumentTypeNameAsync(context.Request, new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings), context.Request.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _evaluator, token))
                .Add(ResultNames.ParentTypeName, property.GetBuilderParentTypeNameAsync(context, _evaluator, token))
                .Build()
                .ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            context.Request.Builder.AddProperties(new PropertyBuilder()
                .WithName(property.Name)
                .WithTypeName(results.GetValue(ResultNames.TypeName).ToString()
                    .FixCollectionTypeName(context.Request.Settings.BuilderNewCollectionTypeName)
                    .FixNullableTypeName(property))
                .WithIsNullable(property.IsNullable)
                .WithIsValueType(property.IsValueType)
                .AddGenericTypeArguments(property.GenericTypeArguments.Select(x => x.ToBuilder().WithTypeName(context.Request.MapTypeName(x.TypeName))))
                .WithParentTypeFullName(results.GetValue(ResultNames.ParentTypeName))
                .AddAttributes(property.Attributes
                    .Where(_ => context.Request.Settings.CopyAttributes)
                    .Select(x => context.Request.MapAttribute(x).ToBuilder()))
                .AddGetterCodeStatements(CreateBuilderPropertyGetterStatements(property, context))
                .AddSetterCodeStatements(await CreateBuilderPropertySetterStatements(property, context, token).ConfigureAwait(false))
            );
        }

        // Note that we are not checking the result, because the same formattable string (CustomBuilderArgumentType) has already been checked earlier in this class
        // We can simple use Value with bang operator to keep the compiler happy (the value should be a string, and not be null)
        context.Request.Builder.AddFields((await context.Request.SourceModel
            .GetBuilderClassFieldsAsync(context, _evaluator, token).ConfigureAwait(false))
            .Select(x => x.Value!));

        return Result.Success();
    }

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertyGetterStatements(Property property, PipelineContext<BuilderContext> context)
    {
        if (property.HasBackingFieldOnBuilder(context.Request.Settings))
        {
            yield return new StringCodeStatementBuilder($"return _{property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo())};");
        }
    }

    private async Task<IEnumerable<CodeStatementBaseBuilder>> CreateBuilderPropertySetterStatements(Property property, PipelineContext<BuilderContext> context, CancellationToken token)
    {
        var results = new List<CodeStatementBaseBuilder>();
        if (property.HasBackingFieldOnBuilder(context.Request.Settings))
        {
            if (context.Request.Settings.CreateAsObservable)
            {
                var nullSuffix = context.Request.Settings.EnableNullableReferenceTypes && !property.IsValueType
                    ? "!"
                    : string.Empty;
                results.Add(new StringCodeStatementBuilder($"bool hasChanged = !{typeof(EqualityComparer<>).WithoutGenerics()}<{(await property.GetBuilderArgumentTypeNameAsync(context.Request, new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings), context.Request.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _evaluator, token).ConfigureAwait(false)).Value}>.Default.Equals(_{property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo())}{nullSuffix}, value{nullSuffix});"));
            }

            results.Add(new StringCodeStatementBuilder($"_{property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo())} = value{property.GetNullCheckSuffix("value", context.Request.Settings.AddNullChecks, context.Request.SourceModel)};"));

            if (context.Request.Settings.CreateAsObservable)
            {
                results.Add(new StringCodeStatementBuilder($"if (hasChanged) HandlePropertyChanged(nameof({property.Name}));"));
            }
        }

        return results;
    }
}
