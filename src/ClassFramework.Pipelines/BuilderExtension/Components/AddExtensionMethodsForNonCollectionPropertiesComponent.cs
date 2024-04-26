﻿namespace ClassFramework.Pipelines.BuilderExtension.Components;

public class AddExtensionMethodsForNonCollectionPropertiesComponentBuilder : IBuilderExtensionComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddExtensionMethodsForNonCollectionPropertiesComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<BuilderExtensionContext, IConcreteTypeBuilder> Build()
        => new AddExtensionMethodsForNonCollectionPropertiesComponent(_formattableStringParser);
}

public class AddExtensionMethodsForNonCollectionPropertiesComponent : IPipelineComponent<BuilderExtensionContext, IConcreteTypeBuilder>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddExtensionMethodsForNonCollectionPropertiesComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result> Process(PipelineContext<BuilderExtensionContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (string.IsNullOrEmpty(context.Request.Settings.SetMethodNameFormatString))
        {
            return Task.FromResult(Result.Continue());
        }

        foreach (var property in context.Request.GetSourceProperties().Where(x => !x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var parentChildContext = new ParentChildContext<PipelineContext<BuilderExtensionContext, IConcreteTypeBuilder>, Property>(context, property, context.Request.Settings);

            var results = context.Request.GetResultsForBuilderNonCollectionProperties(property, parentChildContext, _formattableStringParser);

            var error = Array.Find(results, x => !x.Result.IsSuccessful());
            if (error is not null)
            {
                // Error in formattable string parsing
                return Task.FromResult<Result>(error.Result);
            }

            var returnType = $"{results.First(x => x.Name == "Namespace").Result.Value!.ToString().AppendWhenNotNullOrEmpty(".")}{results.First(x => x.Name == "BuilderName").Result.Value}{context.Request.SourceModel.GetGenericTypeArgumentsString()}";

            var builder = new MethodBuilder()
                .WithName(results.First(x => x.Name == "MethodName").Result.Value!)
                .WithReturnTypeName("T")
                .WithStatic()
                .WithExtensionMethod()
                .AddGenericTypeArguments("T")
                .AddGenericTypeArgumentConstraints($"where T : {returnType}")
                .AddParameter("instance", "T")
                .AddParameters(context.Request.CreateParameterForBuilder(property, results.First(x => x.Name == "TypeName").Result.Value!));

            context.Request.AddNullChecks(builder, results);

            builder.AddStringCodeStatements
            (
                results.First(x => x.Name == "BuilderWithExpression").Result.Value!,
                "return instance;"
            );

            context.Response.AddMethods(builder);
        }

        return Task.FromResult(Result.Continue());
    }
}
