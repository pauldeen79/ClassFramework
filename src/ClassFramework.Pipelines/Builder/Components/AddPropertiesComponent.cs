namespace ClassFramework.Pipelines.Builder.Components;

public class AddPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>, IOrderContainer
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public int Order => PipelineStage.Process;

    public async Task<Result> ExecuteAsync(BuilderContext context, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.IsAbstractBuilder)
        {
            return Result.Continue();
        }

        foreach (var property in context.GetSourceProperties())
        {
            var results = await new AsyncResultDictionaryBuilder<GenericFormattableString>()
                .Add(ResultNames.TypeName, () => property.GetBuilderArgumentTypeNameAsync(context, new ParentChildContext<BuilderContext, Property>(context, property, context.Settings), context.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _evaluator, token))
                .Add(ResultNames.ParentTypeName, () => property.GetBuilderParentTypeNameAsync(context, _evaluator, token))
                .Build()
                .ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            context.Builder.AddProperties(new PropertyBuilder()
                .WithName(property.Name)
                .WithTypeName(results.GetValue(ResultNames.TypeName).ToString()
                    .FixCollectionTypeName(context.Settings.BuilderNewCollectionTypeName)
                    .FixNullableTypeName(property))
                .WithIsNullable(property.IsNullable)
                .WithIsValueType(property.IsValueType)
                .AddGenericTypeArguments(property.GenericTypeArguments.Select(x => x.ToBuilder().WithTypeName(context.MapTypeName(x.TypeName))))
                .WithParentTypeFullName(results.GetValue(ResultNames.ParentTypeName))
                .AddAttributes(property.Attributes
                    .Where(_ => context.Settings.CopyAttributes)
                    .Select(x => context.MapAttribute(x).ToBuilder()))
                .AddGetterCodeStatements(CreateBuilderPropertyGetterStatements(property, context))
                .AddSetterCodeStatements(await CreateBuilderPropertySetterStatements(property, context, token).ConfigureAwait(false))
            );
        }

        // Note that we are not checking the result, because the same formattable string (CustomBuilderArgumentType) has already been checked earlier in this class
        // We can simple use Value with bang operator to keep the compiler happy (the value should be a string, and not be null)
        context.Builder.AddFields((await context.SourceModel
            .GetBuilderClassFieldsAsync(context, _evaluator, token).ConfigureAwait(false))
            .Select(x => x.Value!));

        return Result.Success();
    }

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertyGetterStatements(Property property, BuilderContext context)
    {
        if (property.HasBackingFieldOnBuilder(context.Settings))
        {
            yield return new StringCodeStatementBuilder($"return _{property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo())};");
        }
    }

    private async Task<IEnumerable<CodeStatementBaseBuilder>> CreateBuilderPropertySetterStatements(Property property, BuilderContext context, CancellationToken token)
    {
        var results = new List<CodeStatementBaseBuilder>();
        if (property.HasBackingFieldOnBuilder(context.Settings))
        {
            if (context.Settings.CreateAsObservable)
            {
                var nullSuffix = context.Settings.EnableNullableReferenceTypes && !property.IsValueType
                    ? "!"
                    : string.Empty;
                results.Add(new StringCodeStatementBuilder($"bool hasChanged = !{typeof(EqualityComparer<>).WithoutGenerics()}<{(await property.GetBuilderArgumentTypeNameAsync(context, new ParentChildContext<BuilderContext, Property>(context, property, context.Settings), context.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _evaluator, token).ConfigureAwait(false)).Value}>.Default.Equals(_{property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo())}{nullSuffix}, value{nullSuffix});"));
            }

            results.Add(new StringCodeStatementBuilder($"_{property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo())} = value{property.GetNullCheckSuffix("value", context.Settings.AddNullChecks, context.SourceModel)};"));

            if (context.Settings.CreateAsObservable)
            {
                results.Add(new StringCodeStatementBuilder($"if (hasChanged) HandlePropertyChanged(nameof({property.Name}));"));
            }
        }

        return results;
    }
}
