﻿namespace ClassFramework.Pipelines.Builder.Features;

public class AddFluentMethodsForNonCollectionPropertiesFeatureBuilder : IBuilderFeatureBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddFluentMethodsForNonCollectionPropertiesFeatureBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineFeature<IConcreteTypeBuilder, BuilderContext> Build()
        => new AddFluentMethodsForNonCollectionPropertiesFeature(_formattableStringParser);
}

public class AddFluentMethodsForNonCollectionPropertiesFeature : IPipelineFeature<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddFluentMethodsForNonCollectionPropertiesFeature(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Context.Settings.SetMethodNameFormatString))
        {
            return Result.Continue<IConcreteTypeBuilder>();
        }

        foreach (var property in context.Context.GetSourceProperties().Where(x => context.Context.IsValidForFluentMethod(x) && !x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderContext>, Property>(context, property, context.Context.Settings);

            var results = context.Context.GetResultsForBuilderNonCollectionProperties(property, parentChildContext, _formattableStringParser);

            var error = Array.Find(results, x => !x.Result.IsSuccessful());
            if (error is not null)
            {
                // Error in formattable string parsing
                return Result.FromExistingResult<IConcreteTypeBuilder>(error.Result);
            }

            var builder = new MethodBuilder()
                .WithName(results.First(x => x.Name == "MethodName").Result.Value!)
                .WithReturnTypeName(context.Context.IsBuilderForAbstractEntity
                      ? $"TBuilder{context.Context.SourceModel.GetGenericTypeArgumentsString()}"
                      : $"{results.First(x => x.Name == "Namespace").Result.Value.AppendWhenNotNullOrEmpty(".")}{results.First(x => x.Name == "BuilderName").Result.Value}{context.Context.SourceModel.GetGenericTypeArgumentsString()}")
                .AddParameters
                (
                    new ParameterBuilder()
                        .WithName(property.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()))
                        .WithTypeName(results.First(x => x.Name == "TypeName").Result.Value!)
                        .WithIsNullable(property.IsNullable)
                        .WithIsValueType(property.IsValueType)
                        .WithDefaultValue(GetMetadata(context, property).GetValue<object?>(MetadataNames.CustomBuilderWithDefaultPropertyValue, () => null))
                );

            if (context.Context.Settings.AddNullChecks)
            {
                var nullCheckStatement = results.First(x => x.Name == "ArgumentNullCheck").Result.Value!;
                if (!string.IsNullOrEmpty(nullCheckStatement))
                {
                    builder.AddStringCodeStatements(nullCheckStatement);
                }
            }

            builder.AddStringCodeStatements
            (
                results.First(x => x.Name == "BuilderWithExpression").Result.Value!,
                $"return {GetReturnValue(context.Context)};"
            );

            context.Model.AddMethods(builder);
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }

    private static IEnumerable<Metadata> GetMetadata(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, Property property)
        => context.Context.GetMappingMetadata(property.TypeName);

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, BuilderContext>> ToBuilder()
        => new AddFluentMethodsForNonCollectionPropertiesFeatureBuilder(_formattableStringParser);

    private static string GetReturnValue(BuilderContext context)
    {
        if (context.IsBuilderForAbstractEntity)
        {
            return "(TBuilder)this";
        }

        return "this";
    }
}
