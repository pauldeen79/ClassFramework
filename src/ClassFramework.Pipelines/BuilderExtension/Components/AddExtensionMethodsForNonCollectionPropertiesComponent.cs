namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class AddExtensionMethodsForNonCollectionPropertiesComponentBuilder(IFormattableStringParser formattableStringParser) : IBuilderExtensionComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public IPipelineComponent<BuilderExtensionContext> Build()
        => new AddExtensionMethodsForNonCollectionPropertiesComponent(_formattableStringParser);
}

public class AddExtensionMethodsForNonCollectionPropertiesComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<BuilderExtensionContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> ProcessAsync(PipelineContext<BuilderExtensionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Request.Settings.SetMethodNameFormatString))
        {
            return Task.FromResult(Result.Success());
        }

        foreach (var property in context.Request.GetSourceProperties().Where(x => !x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = new ParentChildContext<PipelineContext<BuilderExtensionContext>, Property>(context, property, context.Request.Settings);

            var results = context.Request.GetResultsForBuilderNonCollectionProperties(property, parentChildContext, _formattableStringParser);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return Task.FromResult<Result>(error);
            }

            var returnType = $"{results["Namespace"].Value!.ToString().AppendWhenNotNullOrEmpty(".")}{results["BuilderName"].Value}{context.Request.SourceModel.GetGenericTypeArgumentsString()}";

            var builder = new MethodBuilder()
                .WithName(results["MethodName"].Value!)
                .WithReturnTypeName("T")
                .WithStatic()
                .WithExtensionMethod()
                .AddGenericTypeArguments("T")
                .AddGenericTypeArgumentConstraints($"where T : {returnType}")
                .AddParameter("instance", "T")
                .AddParameters(context.Request.CreateParameterForBuilder(property, results["TypeName"].Value!));

            context.Request.AddNullChecks(builder, results);

            builder.AddStringCodeStatements
            (
                results["BuilderWithExpression"].Value!,
                "return instance;"
            );

            context.Request.Builder.AddMethods(builder);
        }

        return Task.FromResult(Result.Success());
    }
}
