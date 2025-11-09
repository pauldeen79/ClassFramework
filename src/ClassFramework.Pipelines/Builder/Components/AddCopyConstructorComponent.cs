namespace ClassFramework.Pipelines.Builder.Components;

public class AddCopyConstructorComponent(IExpressionEvaluator evaluator, ICsharpExpressionDumper csharpExpressionDumper) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));
    private readonly ICsharpExpressionDumper _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        if (!context.Settings.AddCopyConstructor)
        {
            return Result.Continue();
        }

        if (context.Settings.EnableBuilderInheritance
            && context.IsAbstractBuilder
            && !context.Settings.IsForAbstractBuilder)
        {
            response.AddConstructors(CreateInheritanceCopyConstructor(context));
        }
        else
        {
            var copyConstructorResult = await CreateCopyConstructorAsync(context, token).ConfigureAwait(false);
            if (!copyConstructorResult.IsSuccessful())
            {
                return copyConstructorResult;
            }

            response.AddConstructors(copyConstructorResult.Value!);
        }

        return Result.Success();
    }

    private async Task<Result<ConstructorBuilder>> CreateCopyConstructorAsync(GenerateBuilderCommand context, CancellationToken token)
    {
        var results = await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add("NullCheck.Source", () => _evaluator.EvaluateInterpolatedStringAsync("{SourceNullCheck()}", context.FormatProvider, context, token))
            .Add(ResultNames.Name, () => _evaluator.EvaluateInterpolatedStringAsync(context.Settings.EntityNameFormatString, context.FormatProvider, context, token))
            .Add(ResultNames.Namespace, () => context.GetMappingMetadata(context.SourceModel.GetFullName()).GetGenericFormattableStringAsync(MetadataNames.CustomEntityNamespace, _evaluator.EvaluateInterpolatedStringAsync(context.Settings.EntityNamespaceFormatString, context.FormatProvider, context, token)))
            .Build()
            .ConfigureAwait(false);

        var error = results.GetError();
        if (error is not null)
        {
            // Error in formattable string parsing
            return Result.FromExistingResult<ConstructorBuilder>(error);
        }

        var initializationCodeResults = await GetInitializationCodeResultsAsync(context, token).ConfigureAwait(false);
        var initializationCodeErrorResult = Array.Find(initializationCodeResults, x => !x.Result.IsSuccessful());
        if (initializationCodeErrorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(initializationCodeErrorResult.Result);
        }

        var constructorInitializerResults = await GetConstructorInitializerResultsAsync(context, token).ConfigureAwait(false);
        var initializerErrorResult = Array.Find(constructorInitializerResults, x => !x.Result.IsSuccessful());
        if (initializerErrorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(initializerErrorResult.Result);
        }

        var name = results.GetValue(ResultNames.Name).ToString();
        var nsPlusPrefix = results.GetValue(ResultNames.Namespace).ToString().AppendWhenNotNullOrEmpty(".");

        return Result.Success(new ConstructorBuilder()
            .WithChainCall(await CreateBuilderClassCopyConstructorChainCallAsync(context.SourceModel, context.Settings).ConfigureAwait(false))
            .WithProtected(context.IsBuilderForAbstractEntity)
            .AddCodeStatements
            (
                new string[] { results.GetValue("NullCheck.Source") }.Where(x => !string.IsNullOrEmpty(x))
            )
            .AddParameters
            (
                new ParameterBuilder()
                    .WithName("source")
                    .WithTypeName($"{nsPlusPrefix}{name}{context.SourceModel.GetGenericTypeArgumentsString()}")
            )
            .AddParameters
            (
                context.GetMappingMetadata(context.SourceModel.GetFullName())
                    .GetValues<Parameter>(MetadataNames.CustomBuilderCopyConstructorParameter)
                    .Select(x => x.ToBuilder())
            )
            .AddCodeStatements(constructorInitializerResults.Select(x => $"{x.Name} = {x.Result.Value};"))
            .AddCodeStatements(initializationCodeResults.Select(x => $"{GetSourceExpression(x.Result.Value!, x.Property, context)};"))
        );
    }

    private async Task<ConstructorPropertyInitializerItem[]> GetInitializationCodeResultsAsync(GenerateBuilderCommand context, CancellationToken token)
    {
        var results = new List<ConstructorPropertyInitializerItem>();

        foreach (var property in context.GetSourceProperties()
            .Where(x => !(x.TypeName.FixTypeName().IsCollectionTypeName()
                    && context.GetMappingMetadata(x.TypeName).Any(y => y.Name == MetadataNames.CustomBuilderConstructorInitializeExpression))))
        {
            var result = await CreateBuilderInitializationCodeAsync(property, context, token).ConfigureAwait(false);

            results.Add(new ConstructorPropertyInitializerItem(property, result));

            if (!result.IsSuccessful())
            {
                break;
            }
        }

        return results.ToArray();
    }

    private async Task<ConstructorPropertyNameInitializerItem[]> GetConstructorInitializerResultsAsync(GenerateBuilderCommand context, CancellationToken cancellationToken)
    {
        var results = new List<ConstructorPropertyNameInitializerItem>();

        foreach (var property in context.GetSourceProperties()
            .Where(x => x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var name = property.GetBuilderMemberName(context.Settings, context.FormatProvider.ToCultureInfo());
            var result = await property.GetBuilderConstructorInitializerAsync(
                context,
                new ParentChildContext<GenerateBuilderCommand, Property>(context, property, context.Settings),
                context.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName),
                context.Settings.BuilderNewCollectionTypeName,
                MetadataNames.CustomBuilderConstructorInitializeExpression,
                _evaluator,
                cancellationToken).ConfigureAwait(false);

            results.Add(new ConstructorPropertyNameInitializerItem(name, result));
            
            if (!result.IsSuccessful())
            {
                break;
            }
        }

        return results.ToArray();
    }

    private Task<Result<GenericFormattableString>> CreateBuilderInitializationCodeAsync(Property property, GenerateBuilderCommand context, CancellationToken token)
        => _evaluator.EvaluateInterpolatedStringAsync
        (
            ProcessCreateBuilderInitializationCode(context.GetMappingMetadata(property.TypeName)
                .GetStringValue
                (
                    MetadataNames.CustomBuilderConstructorInitializeExpression,
                    () => property.TypeName.FixTypeName().IsCollectionTypeName()
                        ? context.Settings.CollectionInitializationStatementFormatString
                        : context.Settings.NonCollectionInitializationStatementFormatString
                ).Replace(PlaceholderNames.NamePlaceholder, property.Name), property.TypeName.FixTypeName().IsCollectionTypeName()),
            context.FormatProvider,
            new ParentChildContext<GenerateBuilderCommand, Property>(context, property, context.Settings),
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

    private string? GetSourceExpression(string? value, Property sourceProperty, GenerateBuilderCommand context)
    {
        if (value is null || !value.Contains(PlaceholderNames.SourceExpressionPlaceholder))
        {
            return value;
        }

        if (value == PlaceholderNames.SourceExpressionPlaceholder)
        {
            return sourceProperty.Name;
        }

        var sourceExpression = context
            .GetMappingMetadata(sourceProperty.TypeName)
            .GetStringValue(MetadataNames.CustomBuilderSourceExpression, PlaceholderNames.NamePlaceholder);

        if (sourceProperty.TypeName.FixTypeName().IsCollectionTypeName())
        {
            return value
                .Replace(PlaceholderNames.SourceExpressionPlaceholder, $"{sourceProperty.Name}.Select(x => {sourceExpression})")
                .Replace(PlaceholderNames.NamePlaceholder, "x").Replace("[NullableSuffix]", string.Empty)
                .Replace("[ForcedNullableSuffix]", string.Empty).Replace(".Select(x => x)", string.Empty);
        }

        var suffix = sourceProperty.GetSuffix(context.Settings.EnableNullableReferenceTypes, _csharpExpressionDumper, context);
        return value
            .Replace($"source.{PlaceholderNames.SourceExpressionPlaceholder}", $"{sourceExpression.Replace(PlaceholderNames.NamePlaceholder, "source." + sourceProperty.Name)}")
            .Replace(PlaceholderNames.NamePlaceholder, sourceProperty.Name)
            .Replace("[NullableSuffix]", suffix)
            .Replace("[ForcedNullableSuffix]", string.IsNullOrEmpty(suffix) ? string.Empty : "!");
    }

    private static async Task<string> CreateBuilderClassCopyConstructorChainCallAsync(IType instance, PipelineSettings settings)
        => (await instance.GetCustomValueForInheritedClassAsync(settings.EnableInheritance, _ => Task.FromResult(Result.Success<GenericFormattableString>("base(source)"))).ConfigureAwait(false)).Value!; //note that the delegate always returns success, so we can simply use the Value here

    private static ConstructorBuilder CreateInheritanceCopyConstructor(GenerateBuilderCommand context)
    {
        var typeName = context.SourceModel.GetFullName();

        return new ConstructorBuilder()
            .WithChainCall("base(source)")
            .WithProtected(context.IsBuilderForAbstractEntity)
            .AddParameters
            (
                new ParameterBuilder()
                    .WithName("source")
                    .WithTypeName($"{typeName}{context.SourceModel.GetGenericTypeArgumentsString()}")
            )
            .AddParameters
            (
                context.GetMappingMetadata(context.SourceModel.GetFullName())
                    .GetValues<Parameter>(MetadataNames.CustomBuilderCopyConstructorParameter)
                    .Select(x => x.ToBuilder())
            );
    }

    private sealed class ConstructorPropertyInitializerItem
    {
        public ConstructorPropertyInitializerItem(Property property, Result<GenericFormattableString> result)
        {
            Property = property;
            Result = result;
        }

        public Property Property { get; }
        public Result<GenericFormattableString> Result { get; }
    }

    private sealed class ConstructorPropertyNameInitializerItem
    {
        public ConstructorPropertyNameInitializerItem(string name, Result<GenericFormattableString> result)
        {
            Name = name;
            Result = result;
        }

        public string Name { get; }
        public Result<GenericFormattableString> Result { get; }
    }
}
