﻿namespace ClassFramework.Pipelines.Builder.Features;

public class AddCopyConstructorComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;
    private readonly ICsharpExpressionDumper _csharpExpressionDumper;

    public AddCopyConstructorComponentBuilder(IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
        _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));
    }

    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build()
        => new AddCopyConstructorComponent(_formattableStringParser, _csharpExpressionDumper);
}

public class AddCopyConstructorComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;
    private readonly ICsharpExpressionDumper _csharpExpressionDumper;

    public AddCopyConstructorComponent(IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
        _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));
    }

    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.AddCopyConstructor)
        {
            return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
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
                return Task.FromResult(Result.FromExistingResult<IConcreteTypeBuilder>(copyConstructorResult));
            }

            context.Model.AddConstructors(copyConstructorResult.Value!);
        }

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }

    private Result<ConstructorBuilder> CreateCopyConstructor(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        var resultSetBuilder = new NamedResultSetBuilder<FormattableStringParserResult>();
        resultSetBuilder.Add("NullCheck.Source", () => _formattableStringParser.Parse("{NullCheck.Source}", context.Context.FormatProvider, context));
        resultSetBuilder.Add(NamedResults.Name, () => _formattableStringParser.Parse(context.Context.Settings.EntityNameFormatString, context.Context.FormatProvider, context));
        resultSetBuilder.Add(NamedResults.Namespace, () => context.Context.GetMappingMetadata(context.Context.SourceModel.GetFullName()).GetFormattableStringParserResult(MetadataNames.CustomEntityNamespace, () => _formattableStringParser.Parse(context.Context.Settings.EntityNamespaceFormatString, context.Context.FormatProvider, context)));
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
        var nsPlusPrefix = context.Context.Settings.InheritFromInterfaces
            ? string.Empty
            : results.First(x => x.Name == NamedResults.Namespace).Result.Value!.ToString().AppendWhenNotNullOrEmpty(".");

        return Result.Success(new ConstructorBuilder()
            .WithChainCall(CreateBuilderClassCopyConstructorChainCall(context.Context.SourceModel, context.Context.Settings))
            .WithProtected(context.Context.IsBuilderForAbstractEntity)
            .AddStringCodeStatements
            (
                new[] { results.First(x => x.Name == "NullCheck.Source").Result.Value!.ToString() }.Where(x => !string.IsNullOrEmpty(x))
            )
            .AddParameters
            (
                new ParameterBuilder()
                    .WithName("source")
                    .WithTypeName($"{nsPlusPrefix}{name}{context.Context.SourceModel.GetGenericTypeArgumentsString()}")
            )
            .AddParameters
            (
                context.Context.GetMappingMetadata(context.Context.SourceModel.GetFullName())
                    .GetValues<Parameter>(MetadataNames.CustomBuilderCopyConstructorParameter)
                    .Select(x => x.ToBuilder())
            )
            .AddStringCodeStatements(constructorInitializerResults.Select(x => $"{x.Item1} = {x.Item2.Value};"))
            .AddStringCodeStatements(initializationCodeResults.Select(x => $"{GetSourceExpression(x.Item2.Value!, x.Item1, context)};"))
        );
    }

    private static string FixEntityName(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, string name, string fullName)
    {
        if (context.Context.Settings.InheritFromInterfaces)
        {
            return context.Context.MapTypeName(fullName, MetadataNames.CustomEntityInterfaceTypeName);
        }

        return name;
    }

    private Tuple<Property, Result<FormattableStringParserResult>>[] GetInitializationCodeResults(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
        => context.Context.SourceModel.Properties
            .Where(x => context.Context.SourceModel.IsMemberValidForBuilderClass(x, context.Context.Settings) && !(x.TypeName.FixTypeName().IsCollectionTypeName() && context.Context.GetMappingMetadata(x.TypeName).Any(y => y.Name == MetadataNames.CustomBuilderConstructorInitializeExpression)))
            .Select(x => new Tuple<Property, Result<FormattableStringParserResult>>
            (
                x,
                CreateBuilderInitializationCode(x, context)
            ))
            .TakeWhileWithFirstNonMatching(x => x.Item2.IsSuccessful())
            .ToArray();

    private Tuple<string, Result<FormattableStringParserResult>>[] GetConstructorInitializerResults(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
        => context.Context.SourceModel.Properties
            .Where(x => context.Context.SourceModel.IsMemberValidForBuilderClass(x, context.Context.Settings) && x.TypeName.FixTypeName().IsCollectionTypeName())
            .Select(x => new Tuple<string, Result<FormattableStringParserResult>>
            (
                x.GetBuilderMemberName(context.Context.Settings, context.Context.FormatProvider.ToCultureInfo()),
                x.GetBuilderConstructorInitializer(context.Context, new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, x, context.Context.Settings), context.Context.MapTypeName(x.TypeName, MetadataNames.CustomEntityInterfaceTypeName), context.Context.Settings.BuilderNewCollectionTypeName, MetadataNames.CustomBuilderConstructorInitializeExpression, _formattableStringParser)
            ))
            .TakeWhileWithFirstNonMatching(x => x.Item2.IsSuccessful())
            .ToArray();

    private Result<FormattableStringParserResult> CreateBuilderInitializationCode(Property property, PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
        => _formattableStringParser.Parse
        (
            ProcessCreateBuilderInitializationCode(context.Context.GetMappingMetadata(property.TypeName)
                .GetStringValue
                (
                    MetadataNames.CustomBuilderConstructorInitializeExpression,
                    () => property.TypeName.FixTypeName().IsCollectionTypeName()
                        ? context.Context.Settings.CollectionInitializationStatementFormatString
                        : context.Context.Settings.NonCollectionInitializationStatementFormatString
                ).Replace(PlaceholderNames.NamePlaceholder, property.Name), property.TypeName.FixTypeName().IsCollectionTypeName()),
            context.Context.FormatProvider,
            new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, property, context.Context.Settings)
        );

    private string ProcessCreateBuilderInitializationCode(string result, bool isCollectionTypeName)
    {
        if (!isCollectionTypeName)
        {
            return $"{{BuilderMemberName}} = {result}";
        }

        return result;
    }

    private string? GetSourceExpression(string? value, Property sourceProperty, PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        if (value is null || !value.Contains(PlaceholderNames.SourceExpressionPlaceholder))
        {
            return value;
        }

        if (value == PlaceholderNames.SourceExpressionPlaceholder)
        {
            return sourceProperty.Name;
        }

        var sourceExpression = context.Context
            .GetMappingMetadata(sourceProperty.TypeName)
            .GetStringValue(MetadataNames.CustomBuilderSourceExpression, PlaceholderNames.NamePlaceholder);

        if (sourceProperty.TypeName.FixTypeName().IsCollectionTypeName())
        {
            return value.Replace(PlaceholderNames.SourceExpressionPlaceholder, $"{sourceProperty.Name}.Select(x => {sourceExpression})").Replace(PlaceholderNames.NamePlaceholder, "x").Replace("[NullableSuffix]", string.Empty).Replace("[ForcedNullableSuffix]", string.Empty).Replace(".Select(x => x)", string.Empty);
        }

        var suffix = sourceProperty.GetSuffix(context.Context.Settings.EnableNullableReferenceTypes, _csharpExpressionDumper, context.Context);
        return value
            .Replace($"source.{PlaceholderNames.SourceExpressionPlaceholder}", $"{sourceExpression.Replace(PlaceholderNames.NamePlaceholder, "source." + sourceProperty.Name)}")
            .Replace(PlaceholderNames.NamePlaceholder, sourceProperty.Name)
            .Replace("[NullableSuffix]", suffix)
            .Replace("[ForcedNullableSuffix]", string.IsNullOrEmpty(suffix) ? string.Empty : "!");
    }

    private static string CreateBuilderClassCopyConstructorChainCall(IType instance, PipelineSettings settings)
        => instance.GetCustomValueForInheritedClass(settings.EnableInheritance, _ => Result.Success("base(source)")).Value!; //note that the delegate always returns success, so we can simply use the Value here

    private static ConstructorBuilder CreateInheritanceCopyConstructor(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        var typeName = context.Context.SourceModel.GetFullName();

        if (context.Context.Settings.InheritFromInterfaces)
        {
            typeName = context.Context.MapTypeName(typeName, MetadataNames.CustomEntityInterfaceTypeName);
        }

        return new ConstructorBuilder()
            .WithChainCall("base(source)")
            .WithProtected(context.Context.IsBuilderForAbstractEntity)
            .AddParameters
            (
                new ParameterBuilder()
                    .WithName("source")
                    .WithTypeName($"{typeName}{context.Context.SourceModel.GetGenericTypeArgumentsString()}")
            )
            .AddParameters
            (
                context.Context.GetMappingMetadata(context.Context.SourceModel.GetFullName())
                    .GetValues<Parameter>(MetadataNames.CustomBuilderCopyConstructorParameter)
                    .Select(x => x.ToBuilder())
            );
    }
}
