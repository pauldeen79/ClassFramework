namespace ClassFramework.Pipelines.Builder.Features;

public class AddPropertiesFeatureBuilder : IBuilderFeatureBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddPropertiesFeatureBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineFeature<IConcreteTypeBuilder, BuilderContext> Build()
        => new AddPropertiesFeature(_formattableStringParser);
}

public class AddPropertiesFeature : IPipelineFeature<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddPropertiesFeature(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Context.IsAbstractBuilder)
        {
            return Result.Continue<IConcreteTypeBuilder>();
        }

        foreach (var property in context.Context.SourceModel.Properties.Where(x => context.Context.SourceModel.IsMemberValidForBuilderClass(x, context.Context.Settings)))
        {
            var typeNameResult = property.GetBuilderArgumentTypeName(context.Context, new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, property, context.Context.Settings), context.Context.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _formattableStringParser);

            if (!typeNameResult.IsSuccessful())
            {
                return Result.FromExistingResult<IConcreteTypeBuilder>(typeNameResult);
            }

            var parentTypeNameResult = property.GetBuilderParentTypeName(context, _formattableStringParser);

            context.Model.AddProperties(new PropertyBuilder()
                .WithName(property.Name)
                .WithTypeName(typeNameResult.Value!
                    .FixCollectionTypeName(context.Context.Settings.BuilderNewCollectionTypeName)
                    .FixNullableTypeName(property))
                .WithIsNullable(property.IsNullable)
                .WithIsValueType(property.IsValueType)
                .WithParentTypeFullName(parentTypeNameResult.Value!)
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

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, BuilderContext>> ToBuilder()
        => new AddPropertiesFeatureBuilder(_formattableStringParser);

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertyGetterStatements(Property property, BuilderContext context)
    {
        if (property.HasBackingFieldOnBuilder(context.Settings.AddNullChecks, context.Settings.EnableNullableReferenceTypes, context.Settings.AddBackingFields || context.Settings.CreateAsObservable))
        {
            yield return new StringCodeStatementBuilder().WithStatement($"return _{property.Name.ToPascalCase(context.FormatProvider.ToCultureInfo())};");
        }
    }

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertySetterStatements(Property property, BuilderContext context)
    {
        if (property.HasBackingFieldOnBuilder(context.Settings.AddNullChecks, context.Settings.EnableNullableReferenceTypes, context.Settings.AddBackingFields || context.Settings.CreateAsObservable))
        {
            yield return new StringCodeStatementBuilder().WithStatement($"_{property.Name.ToPascalCase(context.FormatProvider.ToCultureInfo())} = value{property.GetNullCheckSuffix("value", context.Settings.AddNullChecks)};");
            if (context.Settings.CreateAsObservable)
            {
                yield return new StringCodeStatementBuilder().WithStatement($"HandlePropertyChanged(nameof({property.Name}));");
            }
        }
    }
}
