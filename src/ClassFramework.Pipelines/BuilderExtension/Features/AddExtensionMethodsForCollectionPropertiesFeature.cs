﻿namespace ClassFramework.Pipelines.BuilderExtension.Features;

public class AddExtensionMethodsForCollectionPropertiesFeatureBuilder : IBuilderExtensionFeatureBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddExtensionMethodsForCollectionPropertiesFeatureBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext> Build()
        => new AddExtensionMethodsForCollectionPropertiesFeature(_formattableStringParser);
}

public class AddExtensionMethodsForCollectionPropertiesFeature : IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddExtensionMethodsForCollectionPropertiesFeature(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Context.Settings.AddMethodNameFormatString))
        {
            return Result.Continue<IConcreteTypeBuilder>();
        }

        foreach (var property in context.Context.GetSourceProperties().Where(x => x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = CreateParentChildContext(context, property);

            var results = context.Context.GetResultsForBuilderCollectionProperties(property, parentChildContext, _formattableStringParser, GetCodeStatementsForEnumerableOverload(context, property), GetCodeStatementsForArrayOverload(context, property));

            var error = Array.Find(results, x => !x.Result.IsSuccessful());
            if (error is not null)
            {
                // Error in formattable string parsing
                return Result.FromExistingResult<IConcreteTypeBuilder>(error.Result);
            }

            var returnType = $"{results.First(x => x.Name == "Namespace").Result.Value.AppendWhenNotNullOrEmpty(".")}{results.First(x => x.Name == "BuilderName").Result.Value}{context.Context.SourceModel.GetGenericTypeArgumentsString()}";

            context.Model.AddMethods(new MethodBuilder()
                .WithName(results.First(x => x.Name == "AddMethodName").Result.Value!)
                .WithReturnTypeName("T")
                .WithStatic()
                .WithExtensionMethod()
                .AddGenericTypeArguments("T")
                .AddGenericTypeArgumentConstraints($"where T : {returnType}")
                .AddParameter("instance", "T")
                .AddParameters
                (
                    new ParameterBuilder()
                        .WithName(property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()))
                        .WithTypeName(results.First(x => x.Name == "TypeName").Result.Value!.FixCollectionTypeName(typeof(IEnumerable<>).WithoutGenerics()))
                        .WithIsNullable(property.IsNullable)
                        .WithIsValueType(property.IsValueType)
                )
                .AddStringCodeStatements(results.Where(x => x.Name == "EnumerableOverload").Select(x => x.Result.Value!))
            );

            context.Model.AddMethods(new MethodBuilder()
                .WithName(results.First(x => x.Name == "AddMethodName").Result.Value!)
                .WithReturnTypeName("T")
                .WithStatic()
                .WithExtensionMethod()
                .AddGenericTypeArguments("T")
                .AddGenericTypeArgumentConstraints($"where T : {returnType}")
                .AddParameter("instance", "T")
                .AddParameters
                (
                    new ParameterBuilder()
                        .WithName(property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()))
                        .WithTypeName(results.First(x => x.Name == "TypeName").Result.Value!.FixTypeName().ConvertTypeNameToArray())
                        .WithIsParamArray()
                        .WithIsNullable(property.IsNullable)
                        .WithIsValueType(property.IsValueType)
                )
                .AddStringCodeStatements(results.Where(x => x.Name == "ArrayOverload").Select(x => x.Result.Value!))
            );
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, BuilderExtensionContext>> ToBuilder()
        => new AddExtensionMethodsForCollectionPropertiesFeatureBuilder(_formattableStringParser);

    private IEnumerable<Result<string>> GetCodeStatementsForEnumerableOverload(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context, Property property)
    {
        yield return Result.Success(context.Context.Settings.AddNullChecks
            ? $"return instance.Add{property.Name}<T>({property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()}?.ToArray() ?? throw new {typeof(ArgumentNullException).FullName}(nameof({property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()})));"
            : $"return instance.Add{property.Name}<T>({property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()}.ToArray());");
    }

    private IEnumerable<Result<string>> GetCodeStatementsForArrayOverload(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context, Property property)
    {
        if (context.Context.Settings.AddNullChecks)
        {
            var argumentNullCheckResult = _formattableStringParser.Parse
            (
                context.Context.GetMappingMetadata(property.TypeName).GetStringValue(MetadataNames.CustomBuilderArgumentNullCheckExpression, "{NullCheck.Argument}"),
                context.Context.FormatProvider,
                new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext>, Property>(context, property, context.Context.Settings)
            );
            yield return argumentNullCheckResult;
        }

        var builderAddExpressionResult = _formattableStringParser.Parse
        (
            context.Context
                .GetMappingMetadata(property.TypeName)
                .GetStringValue(MetadataNames.CustomBuilderAddExpression, context.Context.Settings.CollectionCopyStatementFormatString),
            context.Context.FormatProvider,
            new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext>, Property>(context, property, context.Context.Settings)
        );

        yield return builderAddExpressionResult;

        yield return Result.Success("return instance;");
    }

    private static ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext>, Property> CreateParentChildContext(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context, Property property)
        => new(context, property, context.Context.Settings);
}
