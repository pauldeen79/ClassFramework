namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class AddExtensionMethodsForNonCollectionPropertiesComponentBuilder : IBuilderExtensionComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddExtensionMethodsForNonCollectionPropertiesComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<IConcreteTypeBuilder, BuilderExtensionContext> Build()
        => new AddExtensionMethodsForNonCollectionPropertiesComponent(_formattableStringParser);
}

public class AddExtensionMethodsForNonCollectionPropertiesComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderExtensionContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddExtensionMethodsForNonCollectionPropertiesComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Context.Settings.SetMethodNameFormatString))
        {
            return Result.Continue<IConcreteTypeBuilder>();
        }

        foreach (var property in context.Context.GetSourceProperties().Where(x => !x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = new ParentChildContext<PipelineContext<IConcreteTypeBuilder, BuilderExtensionContext>, Property>(context, property, context.Context.Settings);

            var results = context.Context.GetResultsForBuilderNonCollectionProperties(property, parentChildContext, _formattableStringParser);

            var error = Array.Find(results, x => !x.Result.IsSuccessful());
            if (error is not null)
            {
                // Error in formattable string parsing
                return Result.FromExistingResult<IConcreteTypeBuilder>(error.Result);
            }

            var returnType = $"{results.First(x => x.Name == "Namespace").Result.Value.AppendWhenNotNullOrEmpty(".")}{results.First(x => x.Name == "BuilderName").Result.Value}{context.Context.SourceModel.GetGenericTypeArgumentsString()}";

            var builder = new MethodBuilder()
                .WithName(results.First(x => x.Name == "MethodName").Result.Value!)
                .WithReturnTypeName("T")
                .WithStatic()
                .WithExtensionMethod()
                .AddGenericTypeArguments("T")
                .AddGenericTypeArgumentConstraints($"where T : {returnType}")
                .AddParameter("instance", "T")
                .AddParameters(context.Context.CreateParameterForBuilder(property, results.First(x => x.Name == "TypeName").Result.Value!));

            context.Context.AddNullChecks(builder, results);

            builder.AddStringCodeStatements
            (
                results.First(x => x.Name == "BuilderWithExpression").Result.Value!,
                "return instance;"
            );

            context.Model.AddMethods(builder);
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }
}
