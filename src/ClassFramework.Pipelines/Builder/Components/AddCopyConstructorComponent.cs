namespace ClassFramework.Pipelines.Builder.Components;

public class AddCopyConstructorComponent(IExpressionEvaluator evaluator, ICsharpExpressionDumper csharpExpressionDumper) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));
    private readonly ICsharpExpressionDumper _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AddCopyConstructor)
        {
            return Result.Success();
        }

        if (context.Request.Settings.EnableBuilderInheritance
            && context.Request.IsAbstractBuilder
            && !context.Request.Settings.IsForAbstractBuilder)
        {
            context.Request.Builder.AddConstructors(CreateInheritanceCopyConstructor(context));
        }
        else
        {
            var copyConstructorResult = await CreateCopyConstructorAsync(context, token).ConfigureAwait(false);
            if (!copyConstructorResult.IsSuccessful())
            {
                return copyConstructorResult;
            }

            context.Request.Builder.AddConstructors(copyConstructorResult.Value!);
        }

        return Result.Success();
    }

    private async Task<Result<ConstructorBuilder>> CreateCopyConstructorAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        var results = await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add("NullCheck.Source", _evaluator.EvaluateInterpolatedStringAsync("{SourceNullCheck()}", context.Request.FormatProvider, context.Request, token))
            .Add(NamedResults.Name, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.EntityNameFormatString, context.Request.FormatProvider, context.Request, token))
            .Add(NamedResults.Namespace, context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomEntityNamespace, _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.EntityNamespaceFormatString, context.Request.FormatProvider, context.Request, token)))
            .Build()
            .ConfigureAwait(false);

        var error = results.GetError();
        if (error is not null)
        {
            // Error in formattable string parsing
            return Result.FromExistingResult<ConstructorBuilder>(error);
        }

        var initializationCodeResults = await GetInitializationCodeResultsAsync(context, token).ConfigureAwait(false);
        var initializationCodeErrorResult = Array.Find(initializationCodeResults, x => !x.Item2.IsSuccessful());
        if (initializationCodeErrorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(initializationCodeErrorResult.Item2);
        }

        var constructorInitializerResults = await GetConstructorInitializerResultsAsync(context).ConfigureAwait(false);
        var initializerErrorResult = Array.Find(constructorInitializerResults, x => !x.Item2.IsSuccessful());
        if (initializerErrorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(initializerErrorResult.Item2);
        }

        var name = results.GetValue(NamedResults.Name).ToString();
        var nsPlusPrefix = results.GetValue(NamedResults.Namespace).ToString().AppendWhenNotNullOrEmpty(".");

        return Result.Success(new ConstructorBuilder()
            .WithChainCall(await CreateBuilderClassCopyConstructorChainCallAsync(context.Request.SourceModel, context.Request.Settings).ConfigureAwait(false))
            .WithProtected(context.Request.IsBuilderForAbstractEntity)
            .AddCodeStatements
            (
                new string[] { results.GetValue("NullCheck.Source") }.Where(x => !string.IsNullOrEmpty(x))
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
            .AddCodeStatements(constructorInitializerResults.Select(x => $"{x.Item1} = {x.Item2.Value};"))
            .AddCodeStatements(initializationCodeResults.Select(x => $"{GetSourceExpression(x.Item2.Value!, x.Item1, context)};"))
        );
    }

    private async Task<Tuple<Property, Result<GenericFormattableString>>[]> GetInitializationCodeResultsAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        var results = new List<Tuple<Property, Result<GenericFormattableString>>>();

        foreach (var property in context.Request.SourceModel.Properties
            .Where(x => context.Request.SourceModel.IsMemberValidForBuilderClass(x, context.Request.Settings)
                    && !(x.TypeName.FixTypeName().IsCollectionTypeName()
                    && context.Request.GetMappingMetadata(x.TypeName).Any(y => y.Name == MetadataNames.CustomBuilderConstructorInitializeExpression))))
        {
            var result = await CreateBuilderInitializationCodeAsync(property, context, token).ConfigureAwait(false);

            results.Add(new Tuple<Property, Result<GenericFormattableString>>(property, result));

            if (!result.IsSuccessful())
            {
                break;
            }
        }

        return results.ToArray();
    }

    private async Task<Tuple<string, Result<GenericFormattableString>>[]> GetConstructorInitializerResultsAsync(PipelineContext<BuilderContext> context)
    {
        var results = new List<Tuple<string, Result<GenericFormattableString>>>();

        foreach (var property in context.Request.SourceModel.Properties
            .Where(x => context.Request.SourceModel.IsMemberValidForBuilderClass(x, context.Request.Settings)
                    && x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var name = property.GetBuilderMemberName(context.Request.Settings, context.Request.FormatProvider.ToCultureInfo());
            var result = await property.GetBuilderConstructorInitializerAsync(
                context.Request,
                new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings),
                context.Request.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName),
                context.Request.Settings.BuilderNewCollectionTypeName,
                MetadataNames.CustomBuilderConstructorInitializeExpression,
                _evaluator).ConfigureAwait(false);

            results.Add(new Tuple<string, Result<GenericFormattableString>>(name, result));
            
            if (!result.IsSuccessful())
            {
                break;
            }
        }

        return results.ToArray();
    }

    private Task<Result<GenericFormattableString>> CreateBuilderInitializationCodeAsync(Property property, PipelineContext<BuilderContext> context, CancellationToken token)
        => _evaluator.EvaluateInterpolatedStringAsync
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
            new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings),
            token
        );

    private static string ProcessCreateBuilderInitializationCode(string result, bool isCollectionTypeName)
    {
        if (!isCollectionTypeName)
        {
            return $"{{property.BuilderMemberName}} = {result}";
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

    private static async Task<string> CreateBuilderClassCopyConstructorChainCallAsync(IType instance, PipelineSettings settings)
        => (await instance.GetCustomValueForInheritedClassAsync(settings.EnableInheritance, _ => Task.FromResult(Result.Success<GenericFormattableString>("base(source)"))).ConfigureAwait(false)).Value!; //note that the delegate always returns success, so we can simply use the Value here

    private static ConstructorBuilder CreateInheritanceCopyConstructor(PipelineContext<BuilderContext> context)
    {
        var typeName = context.Request.SourceModel.GetFullName();

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
