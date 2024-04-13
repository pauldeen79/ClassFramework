namespace ClassFramework.Pipelines.Builder.Features;

public class AddPropertiesComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddPropertiesComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build()
        => new AddPropertiesComponent(_formattableStringParser);
}

public class AddPropertiesComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddPropertiesComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Context.IsAbstractBuilder)
        {
            return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
        }

        foreach (var property in context.Context.SourceModel.Properties.Where(x => context.Context.SourceModel.IsMemberValidForBuilderClass(x, context.Context.Settings)))
        {
            var resultSetBuilder = new NamedResultSetBuilder<string>();
            resultSetBuilder.Add(NamedResults.TypeName, () => property.GetBuilderArgumentTypeName(context.Context, new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, property, context.Context.Settings), context.Context.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _formattableStringParser));
            resultSetBuilder.Add(NamedResults.ParentTypeName, () => property.GetBuilderParentTypeName(context, _formattableStringParser));
            var results = resultSetBuilder.Build();

            var error = Array.Find(results, x => !x.Result.IsSuccessful());
            if (error is not null)
            {
                // Error in formattable string parsing
                return Result.FromExistingResult<IConcreteTypeBuilder>(error.Result);
            }

            context.Model.AddProperties(new PropertyBuilder()
                .WithName(property.Name)
                .WithTypeName(results.First(x => x.Name == NamedResults.TypeName).Result.Value!
                    .FixCollectionTypeName(context.Context.Settings.BuilderNewCollectionTypeName)
                    .FixNullableTypeName(property))
                .WithIsNullable(property.IsNullable)
                .WithIsValueType(property.IsValueType)
                .AddGenericTypeArguments(property.GenericTypeArguments)
                .WithParentTypeFullName(results.First(x => x.Name == NamedResults.ParentTypeName).Result.Value!)
                .AddAttributes(property.Attributes
                    .Where(_ => context.Context.Settings.CopyAttributes)
                    .Select(x => context.Context.MapAttribute(x).ToBuilder()))
                .AddGetterCodeStatements(CreateBuilderPropertyGetterStatements(property, context.Context))
                .AddSetterCodeStatements(CreateBuilderPropertySetterStatements(property, context.Context))
            );
        }

        // Note that we are not checking the result, because the same formattable string (CustomBuilderArgumentType) has already been checked earlier in this class
        // We can simple use GetValueOrThrow to keep the compiler happy (the value should be a string, and not be null)
        context.Model.AddFields(context.Context.SourceModel
            .GetBuilderClassFields(context, _formattableStringParser)
            .Select(x => x.GetValueOrThrow()));

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
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
