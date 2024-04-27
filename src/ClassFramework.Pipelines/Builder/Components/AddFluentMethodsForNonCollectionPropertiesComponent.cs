namespace ClassFramework.Pipelines.Builder.Components;

public class AddFluentMethodsForNonCollectionPropertiesComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddFluentMethodsForNonCollectionPropertiesComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<BuilderContext, IConcreteTypeBuilder> Build()
        => new AddFluentMethodsForNonCollectionPropertiesComponent(_formattableStringParser);
}

public class AddFluentMethodsForNonCollectionPropertiesComponent : IPipelineComponent<BuilderContext, IConcreteTypeBuilder>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddFluentMethodsForNonCollectionPropertiesComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result> Process(PipelineContext<BuilderContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Request.Settings.SetMethodNameFormatString))
        {
            return Task.FromResult(Result.Continue());
        }

        foreach (var property in context.Request.GetSourceProperties().Where(x => context.Request.IsValidForFluentMethod(x) && !x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = new ParentChildContext<PipelineContext<BuilderContext, IConcreteTypeBuilder>, Property>(context, property, context.Request.Settings);

            var results = context.Request.GetResultsForBuilderNonCollectionProperties(property, parentChildContext, _formattableStringParser);

            var error = Array.Find(results, x => !x.Result.IsSuccessful());
            if (error is not null)
            {
                // Error in formattable string parsing
                return Task.FromResult<Result>(error.Result);
            }

            var builder = new MethodBuilder()
                .WithName(results.First(x => x.Name == "MethodName").Result.Value!)
                .WithReturnTypeName(context.Request.IsBuilderForAbstractEntity
                      ? $"TBuilder{context.Request.SourceModel.GetGenericTypeArgumentsString()}"
                      : $"{results.First(x => x.Name == "Namespace").Result.Value!.ToString().AppendWhenNotNullOrEmpty(".")}{results.First(x => x.Name == "BuilderName").Result.Value}{context.Request.SourceModel.GetGenericTypeArgumentsString()}")
                .AddParameters(context.Request.CreateParameterForBuilder(property, results.First(x => x.Name == "TypeName").Result.Value!));

            context.Request.AddNullChecks(builder, results);

            builder.AddStringCodeStatements
            (
                results.First(x => x.Name == "BuilderWithExpression").Result.Value!,
                context.Request.ReturnValueStatementForFluentMethod
            );

            context.Response.AddMethods(builder);
        }

        return Task.FromResult(Result.Continue());
    }
}
