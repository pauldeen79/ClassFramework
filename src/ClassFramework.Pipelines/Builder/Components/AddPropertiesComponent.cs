namespace ClassFramework.Pipelines.Builder.Components;

public class AddPropertiesComponentBuilder(IFormattableStringParser formattableStringParser) : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public IPipelineComponent<BuilderContext> Build()
        => new AddPropertiesComponent(_formattableStringParser);
}

public class AddPropertiesComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> Process(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.IsAbstractBuilder)
        {
            return Task.FromResult(Result.Success());
        }

        foreach (var property in context.Request.SourceModel.Properties.Where(x => context.Request.SourceModel.IsMemberValidForBuilderClass(x, context.Request.Settings)))
        {
            var results = new ResultDictionaryBuilder<FormattableStringParserResult>()
                .Add(NamedResults.TypeName, () => property.GetBuilderArgumentTypeName(context.Request, new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings), context.Request.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _formattableStringParser))
                .Add(NamedResults.ParentTypeName, () => property.GetBuilderParentTypeName(context, _formattableStringParser))
                .Build();

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return Task.FromResult<Result>(error);
            }

            context.Request.Builder.AddProperties(new PropertyBuilder()
                .WithName(property.Name)
                .WithTypeName(results[NamedResults.TypeName].Value!.ToString()
                    .FixCollectionTypeName(context.Request.Settings.BuilderNewCollectionTypeName)
                    .FixNullableTypeName(property))
                .WithIsNullable(property.IsNullable)
                .WithIsValueType(property.IsValueType)
                .AddGenericTypeArguments(property.GenericTypeArguments)
                .WithParentTypeFullName(results[NamedResults.ParentTypeName].Value!)
                .AddAttributes(property.Attributes
                    .Where(_ => context.Request.Settings.CopyAttributes)
                    .Select(x => context.Request.MapAttribute(x).ToBuilder()))
                .AddGetterCodeStatements(CreateBuilderPropertyGetterStatements(property, context))
                .AddSetterCodeStatements(CreateBuilderPropertySetterStatements(property, context))
            );
        }

        // Note that we are not checking the result, because the same formattable string (CustomBuilderArgumentType) has already been checked earlier in this class
        // We can simple use GetValueOrThrow to keep the compiler happy (the value should be a string, and not be null)
        context.Request.Builder.AddFields(context.Request.SourceModel
            .GetBuilderClassFields(context, _formattableStringParser)
            .Select(x => x.GetValueOrThrow()));

        return Task.FromResult(Result.Success());
    }

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertyGetterStatements(Property property, PipelineContext<BuilderContext> context)
    {
        if (property.HasBackingFieldOnBuilder(context.Request.Settings))
        {
            yield return new StringCodeStatementBuilder().WithStatement($"return _{property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo())};");
        }
    }

    private IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertySetterStatements(Property property, PipelineContext<BuilderContext> context)
    {
        if (property.HasBackingFieldOnBuilder(context.Request.Settings))
        {
            if (context.Request.Settings.CreateAsObservable)
            {
                var nullSuffix = context.Request.Settings.EnableNullableReferenceTypes && !property.IsValueType
                    ? "!"
                    : string.Empty;
                yield return new StringCodeStatementBuilder().WithStatement($"bool hasChanged = !{typeof(EqualityComparer<>).WithoutGenerics()}<{property.GetBuilderArgumentTypeName(context.Request, new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings), context.Request.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _formattableStringParser).Value}>.Default.Equals(_{property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo())}{nullSuffix}, value{nullSuffix});");
            }

            yield return new StringCodeStatementBuilder().WithStatement($"_{property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo())} = value{property.GetNullCheckSuffix("value", context.Request.Settings.AddNullChecks, context.Request.SourceModel)};");

            if (context.Request.Settings.CreateAsObservable)
            {
                yield return new StringCodeStatementBuilder().WithStatement($"if (hasChanged) HandlePropertyChanged(nameof({property.Name}));");
            }
        }
    }
}
