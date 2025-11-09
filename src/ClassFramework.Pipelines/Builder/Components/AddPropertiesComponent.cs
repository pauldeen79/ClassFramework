namespace ClassFramework.Pipelines.Builder.Components;

public class AddPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        if (context.IsAbstractBuilder)
        {
            return Result.Continue();
        }

        foreach (var property in context.GetSourceProperties())
        {
            var results = await new AsyncResultDictionaryBuilder<GenericFormattableString>()
                .Add(ResultNames.TypeName, () => property.GetBuilderArgumentTypeNameAsync(context, new ParentChildContext<GenerateBuilderCommand, Property>(context, property, context.Settings), context.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _evaluator, token))
                .Add(ResultNames.ParentTypeName, () => property.GetBuilderParentTypeNameAsync(context, _evaluator, token))
                .Build()
                .ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            response.AddProperties(new PropertyBuilder()
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
                .AddSetterCodeStatements(await CreateBuilderPropertySetterStatementsAsync(property, context, token).ConfigureAwait(false))
            );
        }

        // Note that we are not checking the result, because the same formattable string (CustomBuilderArgumentType) has already been checked earlier in this class
        // We can simple use Value with bang operator to keep the compiler happy (the value should be a string, and not be null)
        response.AddFields((await context.SourceModel
            .GetBuilderClassFieldsAsync(context, _evaluator, token).ConfigureAwait(false))
            .Select(x => x.Value!));

        return Result.Success();
    }

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertyGetterStatements(Property property, GenerateBuilderCommand context)
    {
        if (property.HasBackingFieldOnBuilder(context.Settings))
        {
            yield return new StringCodeStatementBuilder($"return _{property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo())};");
        }
    }

    private async Task<IEnumerable<CodeStatementBaseBuilder>> CreateBuilderPropertySetterStatementsAsync(Property property, GenerateBuilderCommand context, CancellationToken token)
    {
        var results = new List<CodeStatementBaseBuilder>();
        if (property.HasBackingFieldOnBuilder(context.Settings))
        {
            if (context.Settings.CreateAsObservable)
            {
                var nullSuffix = context.Settings.EnableNullableReferenceTypes && !property.IsValueType
                    ? "!"
                    : string.Empty;
                results.Add(new StringCodeStatementBuilder($"bool hasChanged = !{typeof(EqualityComparer<>).WithoutGenerics()}<{(await property.GetBuilderArgumentTypeNameAsync(context, new ParentChildContext<GenerateBuilderCommand, Property>(context, property, context.Settings), context.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _evaluator, token).ConfigureAwait(false)).Value}>.Default.Equals(_{property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo())}{nullSuffix}, value{nullSuffix});"));
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
