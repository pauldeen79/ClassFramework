namespace ClassFramework.Pipelines.Builder.Features;

public class AddPropertiesComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddPropertiesComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<BuilderContext, IConcreteTypeBuilder> Build()
        => new AddPropertiesComponent(_formattableStringParser);
}

public class AddPropertiesComponent : IPipelineComponent<BuilderContext, IConcreteTypeBuilder>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddPropertiesComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result> Process(PipelineContext<BuilderContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.IsAbstractBuilder)
        {
            return Task.FromResult(Result.Continue());
        }

        foreach (var property in context.Request.SourceModel.Properties.Where(x => context.Request.SourceModel.IsMemberValidForBuilderClass(x, context.Request.Settings)))
        {
            var resultSetBuilder = new NamedResultSetBuilder<FormattableStringParserResult>();
            resultSetBuilder.Add(NamedResults.TypeName, () => property.GetBuilderArgumentTypeName(context.Request, new ParentChildContext<PipelineContext<BuilderContext, IConcreteTypeBuilder>, Property>(context, property, context.Request.Settings), context.Request.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _formattableStringParser));
            resultSetBuilder.Add(NamedResults.ParentTypeName, () => property.GetBuilderParentTypeName(context, _formattableStringParser));
            var results = resultSetBuilder.Build();

            var error = Array.Find(results, x => !x.Result.IsSuccessful());
            if (error is not null)
            {
                // Error in formattable string parsing
                return Task.FromResult<Result>(error.Result);
            }

            context.Response.AddProperties(new PropertyBuilder()
                .WithName(property.Name)
                .WithTypeName(results.First(x => x.Name == NamedResults.TypeName).Result.Value!.ToString()
                    .FixCollectionTypeName(context.Request.Settings.BuilderNewCollectionTypeName)
                    .FixNullableTypeName(property))
                .WithIsNullable(property.IsNullable)
                .WithIsValueType(property.IsValueType)
                .AddGenericTypeArguments(property.GenericTypeArguments)
                .WithParentTypeFullName(results.First(x => x.Name == NamedResults.ParentTypeName).Result.Value!)
                .AddAttributes(property.Attributes
                    .Where(_ => context.Request.Settings.CopyAttributes)
                    .Select(x => context.Request.MapAttribute(x).ToBuilder()))
                .AddGetterCodeStatements(CreateBuilderPropertyGetterStatements(property, context.Request))
                .AddSetterCodeStatements(CreateBuilderPropertySetterStatements(property, context.Request))
            );
        }

        // Note that we are not checking the result, because the same formattable string (CustomBuilderArgumentType) has already been checked earlier in this class
        // We can simple use GetValueOrThrow to keep the compiler happy (the value should be a string, and not be null)
        context.Response.AddFields(context.Request.SourceModel
            .GetBuilderClassFields(context, _formattableStringParser)
            .Select(x => x.GetValueOrThrow()));

        return Task.FromResult(Result.Continue());
    }

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertyGetterStatements(Property property, BuilderContext context)
    {
        if (property.HasBackingFieldOnBuilder(context.Settings))
        {
            yield return new StringCodeStatementBuilder().WithStatement($"return _{property.Name.ToPascalCase(context.FormatProvider.ToCultureInfo())};");
        }
    }

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertySetterStatements(Property property, BuilderContext context)
    {
        if (property.HasBackingFieldOnBuilder(context.Settings))
        {
            yield return new StringCodeStatementBuilder().WithStatement($"_{property.Name.ToPascalCase(context.FormatProvider.ToCultureInfo())} = value{property.GetNullCheckSuffix("value", context.Settings.AddNullChecks, context.SourceModel)};");
            if (context.Settings.CreateAsObservable)
            {
                yield return new StringCodeStatementBuilder().WithStatement($"HandlePropertyChanged(nameof({property.Name}));");
            }
        }
    }
}
