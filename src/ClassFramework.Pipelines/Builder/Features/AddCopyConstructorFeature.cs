namespace ClassFramework.Pipelines.Builder.Features;

public class AddCopyConstructorFeatureBuilder : IBuilderFeatureBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddCopyConstructorFeatureBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineFeature<IConcreteTypeBuilder, BuilderContext> Build()
        => new AddCopyConstructorFeature(_formattableStringParser);
}

public class AddCopyConstructorFeature : IPipelineFeature<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddCopyConstructorFeature(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.AddCopyConstructor)
        {
            return Result.Continue<IConcreteTypeBuilder>();
        }

        if (context.Context.Settings.EnableBuilderInheritance
            && context.Context.IsAbstractBuilder
            && !context.Context.Settings.IsForAbstractBuilder)
        {
            context.Model.AddConstructors(CreateInheritanceCopyConstructor(context));
        }
        else
        {
            var copyConstructorResult = CreateCopyConstructor(context);
            if (!copyConstructorResult.IsSuccessful())
            {
                return Result.FromExistingResult<IConcreteTypeBuilder>(copyConstructorResult);
            }

            context.Model.AddConstructors(copyConstructorResult.Value!);
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, BuilderContext>> ToBuilder()
        => new AddCopyConstructorFeatureBuilder(_formattableStringParser);

    private Result<ConstructorBuilder> CreateCopyConstructor(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        var resultSetBuilder = new NamedResultSetBuilder<string>();
        resultSetBuilder.Add("NullCheck.Source", () => _formattableStringParser.Parse("{NullCheck.Source}", context.Context.FormatProvider, context));
        resultSetBuilder.Add("Name", () => _formattableStringParser.Parse(context.Context.Settings.EntityNameFormatString, context.Context.FormatProvider, context));
        resultSetBuilder.Add("Namespace", () => context.Context.SourceModel.Metadata.WithMappingMetadata(context.Context.SourceModel.GetFullName().GetCollectionItemType().WhenNullOrEmpty(context.Context.SourceModel.GetFullName), context.Context.Settings).GetStringResult(MetadataNames.CustomEntityNamespace, () => _formattableStringParser.Parse(context.Context.Settings.EntityNamespaceFormatString, context.Context.FormatProvider, context)));
        var results = resultSetBuilder.Build();

        var error = Array.Find(results, x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            // Error in formattable string parsing
            return Result.FromExistingResult<ConstructorBuilder>(error.Result);
        }

        var initializationCodeResults = GetInitializationCodeResults(context);
        var initializationCodeErrorResult = Array.Find(initializationCodeResults, x => !x.Item2.IsSuccessful());
        if (initializationCodeErrorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(initializationCodeErrorResult.Item2);
        }

        var constructorInitializerResults = GetConstructorInitializerResults(context);
        var initializerErrorResult = Array.Find(constructorInitializerResults, x => !x.Item2.IsSuccessful());
        if (initializerErrorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(initializerErrorResult.Item2);
        }

        var name = results.First(x => x.Name == "Name").Result.Value!;
        name = FixEntityName(context, name);

        return Result.Success(new ConstructorBuilder()
            .WithChainCall(CreateBuilderClassCopyConstructorChainCall(context.Context.SourceModel, context.Context.Settings))
            .WithProtected(context.Context.IsBuilderForAbstractEntity)
            .AddStringCodeStatements
            (
                new[] { results.First(x => x.Name == "NullCheck.Source").Result.Value! }.Where(x => !string.IsNullOrEmpty(x))
            )
            .AddParameters
            (
                new ParameterBuilder()
                    .WithName("source")
                    .WithTypeName($"{results.First(x => x.Name == "Namespace").Result.Value.AppendWhenNotNullOrEmpty(".")}{name}{context.Context.SourceModel.GetGenericTypeArgumentsString()}")
            )
            .AddParameters
            (
                context.Context.SourceModel.Metadata
                    .WithMappingMetadata(context.Context.SourceModel.GetFullName().GetCollectionItemType().WhenNullOrEmpty(context.Context.SourceModel.GetFullName), context.Context.Settings)
                    .GetValues<Parameter>(MetadataNames.CustomBuilderCopyConstructorParameter)
                    .Select(x => x.ToBuilder())
            )
            .AddStringCodeStatements(constructorInitializerResults.Select(x => $"{x.Item1} = {x.Item2.Value};"))
            .AddStringCodeStatements(initializationCodeResults.Select(x => $"{GetSourceExpression(x.Item2.Value, x.Item1, context)};"))
        );
    }

    private static string FixEntityName(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, string name)
    {
        if (context.Context.Settings.OriginalValidateArguments == ArgumentValidationType.Shared
            && !context.Context.Settings.EnableBuilderInheritance)
        {
            return $"{name}Base";
        }

        return name;
    }

    private Tuple<Property, Result<string>>[] GetInitializationCodeResults(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
        => context.Context.SourceModel.Properties
            .Where(x => context.Context.SourceModel.IsMemberValidForBuilderClass(x, context.Context.Settings))
            .Select(x => new Tuple<Property, Result<string>>
            (
                x,
                CreateBuilderInitializationCode(x, context)
            ))
            .TakeWhileWithFirstNonMatching(x => x.Item2.IsSuccessful())
            .ToArray();

    private Tuple<string, Result<string>>[] GetConstructorInitializerResults(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
        => context.Context.SourceModel.Properties
            .Where(x => context.Context.SourceModel.IsMemberValidForBuilderClass(x, context.Context.Settings) && x.TypeName.FixTypeName().IsCollectionTypeName())
            .Select(x => new Tuple<string, Result<string>>
            (
                x.GetBuilderMemberName(context.Context.Settings.AddNullChecks, context.Context.Settings.EnableNullableReferenceTypes, context.Context.Settings.OriginalValidateArguments, context.Context.Settings.AddBackingFields, context.Context.FormatProvider.ToCultureInfo()),
                x.GetBuilderConstructorInitializer(context.Context.Settings, context.Context.FormatProvider, new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, x, context.Context.Settings), context.Context.MapTypeName(x.TypeName), context.Context.Settings.BuilderNewCollectionTypeName, _formattableStringParser)
            ))
            .TakeWhileWithFirstNonMatching(x => x.Item2.IsSuccessful())
            .ToArray();

    private Result<string> CreateBuilderInitializationCode(Property property, PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
        => _formattableStringParser.Parse
        (
            property.Metadata
                .WithMappingMetadata(property.TypeName.GetCollectionItemType().WhenNullOrEmpty(property.TypeName), context.Context.Settings)
                .GetStringValue
                (
                    MetadataNames.CustomBuilderConstructorInitializeExpression,
                    () => property.TypeName.FixTypeName().IsCollectionTypeName()
                        ? context.Context.Settings.CollectionInitializationStatementFormatString
                        : context.Context.Settings.NonCollectionInitializationStatementFormatString
                ),
            context.Context.FormatProvider,
            new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, property, context.Context.Settings)
        );

    private static string? GetSourceExpression(string? value, Property sourceProperty, PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        if (value is null || !value.Contains("[SourceExpression]"))
        {
            return value;
        }

        if (value == "[SourceExpression]")
        {
            return sourceProperty.Name;
        }

        var metadata = sourceProperty.Metadata.WithMappingMetadata(sourceProperty.TypeName.GetCollectionItemType().WhenNullOrEmpty(sourceProperty.TypeName), context.Context.Settings);
        var sourceExpression = metadata.GetStringValue(MetadataNames.CustomBuilderSourceExpression, "[Name]");
        return sourceProperty.TypeName.FixTypeName().IsCollectionTypeName()
            ? value.Replace("[SourceExpression]", $"{sourceProperty.Name}.Select(x => {sourceExpression})").Replace("[Name]", "x").Replace("[NullableSuffix]", string.Empty).Replace(".Select(x => x)", string.Empty)
            : value.Replace("[SourceExpression]", sourceExpression).Replace("[Name]", sourceProperty.Name).Replace("[NullableSuffix]", sourceProperty.GetSuffix(context.Context.Settings.EnableNullableReferenceTypes));
    }
    private static string CreateBuilderClassCopyConstructorChainCall(IType instance, PipelineSettings settings)
        => instance.GetCustomValueForInheritedClass(settings.EnableInheritance, _ => Result.Success("base(source)")).Value!; //note that the delegate always returns success, so we can simply use the Value here

    private static ConstructorBuilder CreateInheritanceCopyConstructor(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
        => new ConstructorBuilder()
            .WithChainCall("base(source)")
            .WithProtected(context.Context.IsBuilderForAbstractEntity)
            .AddParameters
            (
                new ParameterBuilder()
                    .WithName("source")
                    .WithTypeName($"{context.Context.SourceModel.GetFullName()}{context.Context.SourceModel.GetGenericTypeArgumentsString()}")
            )
            .AddParameters
            (
                context.Context.SourceModel.Metadata
                    .WithMappingMetadata(context.Context.SourceModel.GetFullName().GetCollectionItemType().WhenNullOrEmpty(context.Context.SourceModel.GetFullName), context.Context.Settings)
                    .GetValues<Parameter>(MetadataNames.CustomBuilderCopyConstructorParameter)
                    .Select(x => x.ToBuilder())
            );
}
