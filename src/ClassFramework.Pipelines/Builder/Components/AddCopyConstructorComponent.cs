namespace ClassFramework.Pipelines.Builder.Components;

public class AddCopyConstructorComponentBuilder(IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper) : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    private readonly ICsharpExpressionDumper _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));

    public IPipelineComponent<BuilderContext> Build()
        => new AddCopyConstructorComponent(_formattableStringParser, _csharpExpressionDumper);
}

public class AddCopyConstructorComponent(IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper) : IPipelineComponent<BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    private readonly ICsharpExpressionDumper _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));

    public Task<Result> Process(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AddCopyConstructor)
        {
            return Task.FromResult(Result.Continue());
        }

        if (context.Request.Settings.EnableBuilderInheritance
            && context.Request.IsAbstractBuilder
            && !context.Request.Settings.IsForAbstractBuilder)
        {
            context.Request.Builder.AddConstructors(CreateInheritanceCopyConstructor(context));
        }
        else
        {
            var copyConstructorResult = CreateCopyConstructor(context);
            if (!copyConstructorResult.IsSuccessful())
            {
                return Task.FromResult<Result>(copyConstructorResult);
            }

            context.Request.Builder.AddConstructors(copyConstructorResult.Value!);
        }

        return Task.FromResult(Result.Continue());
    }

    private Result<ConstructorBuilder> CreateCopyConstructor(PipelineContext<BuilderContext> context)
    {
        var resultSetBuilder = new NamedResultSetBuilder<FormattableStringParserResult>();
        resultSetBuilder.Add("NullCheck.Source", () => _formattableStringParser.Parse("{NullCheck.Source}", context.Request.FormatProvider, context));
        resultSetBuilder.Add(NamedResults.Name, () => _formattableStringParser.Parse(context.Request.Settings.EntityNameFormatString, context.Request.FormatProvider, context));
        resultSetBuilder.Add(NamedResults.Namespace, () => context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetFormattableStringParserResult(MetadataNames.CustomEntityNamespace, () => _formattableStringParser.Parse(context.Request.Settings.EntityNamespaceFormatString, context.Request.FormatProvider, context)));
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

        var name = results.First(x => x.Name == NamedResults.Name).Result.Value!.ToString();
        name = FixEntityName(context, name, $"{results.First(x => x.Name == NamedResults.Namespace).Result.Value!.ToString().AppendWhenNotNullOrEmpty(".")}{name}");
        var nsPlusPrefix = context.Request.Settings.InheritFromInterfaces
            ? string.Empty
            : results.First(x => x.Name == NamedResults.Namespace).Result.Value!.ToString().AppendWhenNotNullOrEmpty(".");

        return Result.Success(new ConstructorBuilder()
            .WithChainCall(CreateBuilderClassCopyConstructorChainCall(context.Request.SourceModel, context.Request.Settings))
            .WithProtected(context.Request.IsBuilderForAbstractEntity)
            .AddStringCodeStatements
            (
                new[] { results.First(x => x.Name == "NullCheck.Source").Result.Value!.ToString() }.Where(x => !string.IsNullOrEmpty(x))
            )
            .AddParameters
            (
                new ParameterBuilder()
                    .WithName("source")
                    .WithTypeName($"{nsPlusPrefix}{name}{context.Request.SourceModel.GetGenericTypeArgumentsString()}")
            )
            .AddParameters
            (
                context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName())
                    .GetValues<Parameter>(MetadataNames.CustomBuilderCopyConstructorParameter)
                    .Select(x => x.ToBuilder())
            )
            .AddStringCodeStatements(constructorInitializerResults.Select(x => $"{x.Item1} = {x.Item2.Value};"))
            .AddStringCodeStatements(initializationCodeResults.Select(x => $"{GetSourceExpression(x.Item2.Value!, x.Item1, context)};"))
        );
    }

    private static string FixEntityName(PipelineContext<BuilderContext> context, string name, string fullName)
    {
        if (context.Request.Settings.InheritFromInterfaces)
        {
            return context.Request.MapTypeName(fullName, MetadataNames.CustomEntityInterfaceTypeName);
        }

        return name;
    }

    private Tuple<Property, Result<FormattableStringParserResult>>[] GetInitializationCodeResults(PipelineContext<BuilderContext> context)
        => context.Request.SourceModel.Properties
            .Where(x => context.Request.SourceModel.IsMemberValidForBuilderClass(x, context.Request.Settings) && !(x.TypeName.FixTypeName().IsCollectionTypeName() && context.Request.GetMappingMetadata(x.TypeName).Any(y => y.Name == MetadataNames.CustomBuilderConstructorInitializeExpression)))
            .Select(x => new Tuple<Property, Result<FormattableStringParserResult>>
            (
                x,
                CreateBuilderInitializationCode(x, context)
            ))
            .TakeWhileWithFirstNonMatching(x => x.Item2.IsSuccessful())
            .ToArray();

    private Tuple<string, Result<FormattableStringParserResult>>[] GetConstructorInitializerResults(PipelineContext<BuilderContext> context)
        => context.Request.SourceModel.Properties
            .Where(x => context.Request.SourceModel.IsMemberValidForBuilderClass(x, context.Request.Settings) && x.TypeName.FixTypeName().IsCollectionTypeName())
            .Select(x => new Tuple<string, Result<FormattableStringParserResult>>
            (
                x.GetBuilderMemberName(context.Request.Settings, context.Request.FormatProvider.ToCultureInfo()),
                x.GetBuilderConstructorInitializer(context.Request, new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, x, context.Request.Settings), context.Request.MapTypeName(x.TypeName, MetadataNames.CustomEntityInterfaceTypeName), context.Request.Settings.BuilderNewCollectionTypeName, MetadataNames.CustomBuilderConstructorInitializeExpression, _formattableStringParser)
            ))
            .TakeWhileWithFirstNonMatching(x => x.Item2.IsSuccessful())
            .ToArray();

    private Result<FormattableStringParserResult> CreateBuilderInitializationCode(Property property, PipelineContext<BuilderContext> context)
        => _formattableStringParser.Parse
        (
            ProcessCreateBuilderInitializationCode(context.Request.GetMappingMetadata(property.TypeName)
                .GetStringValue
                (
                    MetadataNames.CustomBuilderConstructorInitializeExpression,
                    () => property.TypeName.FixTypeName().IsCollectionTypeName()
                        ? context.Request.Settings.CollectionInitializationStatementFormatString
                        : context.Request.Settings.NonCollectionInitializationStatementFormatString
                ).Replace(PlaceholderNames.NamePlaceholder, property.Name), property.TypeName.FixTypeName().IsCollectionTypeName()),
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings)
        );

    private static string ProcessCreateBuilderInitializationCode(string result, bool isCollectionTypeName)
    {
        if (!isCollectionTypeName)
        {
            return $"{{BuilderMemberName}} = {result}";
        }

        return result;
    }

    private string? GetSourceExpression(string? value, Property sourceProperty, PipelineContext<BuilderContext> context)
    {
        if (value is null || !value.Contains(PlaceholderNames.SourceExpressionPlaceholder))
        {
            return value;
        }

        if (value == PlaceholderNames.SourceExpressionPlaceholder)
        {
            return sourceProperty.Name;
        }

        var sourceExpression = context.Request
            .GetMappingMetadata(sourceProperty.TypeName)
            .GetStringValue(MetadataNames.CustomBuilderSourceExpression, PlaceholderNames.NamePlaceholder);

        if (sourceProperty.TypeName.FixTypeName().IsCollectionTypeName())
        {
            return value
                .Replace(PlaceholderNames.SourceExpressionPlaceholder, $"{sourceProperty.Name}.Select(x => {sourceExpression})")
                .Replace(PlaceholderNames.NamePlaceholder, "x").Replace("[NullableSuffix]", string.Empty)
                .Replace("[ForcedNullableSuffix]", string.Empty).Replace(".Select(x => x)", string.Empty);
        }

        var suffix = sourceProperty.GetSuffix(context.Request.Settings.EnableNullableReferenceTypes, _csharpExpressionDumper, context.Request);
        return value
            .Replace($"source.{PlaceholderNames.SourceExpressionPlaceholder}", $"{sourceExpression.Replace(PlaceholderNames.NamePlaceholder, "source." + sourceProperty.Name)}")
            .Replace(PlaceholderNames.NamePlaceholder, sourceProperty.Name)
            .Replace("[NullableSuffix]", suffix)
            .Replace("[ForcedNullableSuffix]", string.IsNullOrEmpty(suffix) ? string.Empty : "!");
    }

    private static string CreateBuilderClassCopyConstructorChainCall(IType instance, PipelineSettings settings)
        => instance.GetCustomValueForInheritedClass(settings.EnableInheritance, _ => Result.Success<FormattableStringParserResult>("base(source)")).Value!; //note that the delegate always returns success, so we can simply use the Value here

    private static ConstructorBuilder CreateInheritanceCopyConstructor(PipelineContext<BuilderContext> context)
    {
        var typeName = context.Request.SourceModel.GetFullName();

        if (context.Request.Settings.InheritFromInterfaces)
        {
            typeName = context.Request.MapTypeName(typeName, MetadataNames.CustomEntityInterfaceTypeName);
        }

        return new ConstructorBuilder()
            .WithChainCall("base(source)")
            .WithProtected(context.Request.IsBuilderForAbstractEntity)
            .AddParameters
            (
                new ParameterBuilder()
                    .WithName("source")
                    .WithTypeName($"{typeName}{context.Request.SourceModel.GetGenericTypeArgumentsString()}")
            )
            .AddParameters
            (
                context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName())
                    .GetValues<Parameter>(MetadataNames.CustomBuilderCopyConstructorParameter)
                    .Select(x => x.ToBuilder())
            );
    }
}
