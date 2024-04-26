﻿namespace ClassFramework.Pipelines.Builder.Features;

public class AbstractBuilderComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AbstractBuilderComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build()
        => new AbstractBuilderComponent(_formattableStringParser);
}

public class AbstractBuilderComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AbstractBuilderComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result<IConcreteTypeBuilder>> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.IsBuilderForAbstractEntity /*&& context.Request.IsAbstractBuilder*/)
        {
            var nameResult = _formattableStringParser.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context);
            if (!nameResult.IsSuccessful())
            {
                return Task.FromResult(Result.FromExistingResult<IConcreteTypeBuilder>(nameResult));
            }

            if (context.Model is not ClassBuilder classBuilder)
            {
                return Task.FromResult(Result.Invalid<IConcreteTypeBuilder>($"You can only create abstract classes. The type of model ({context.Response.GetType().FullName}) is not a ClassBuilder"));
            }

            classBuilder.WithAbstract();

            if (!context.Request.Settings.IsForAbstractBuilder)
            {
                classBuilder
                    .AddGenericTypeArguments("TBuilder", "TEntity")
                    .AddGenericTypeArgumentConstraints($"where TEntity : {context.Request.SourceModel.GetFullName()}")
                    .AddGenericTypeArgumentConstraints($"where TBuilder : {nameResult.Value}<TBuilder, TEntity>")
                    .WithAbstract();
            }
        }

        return Task.FromResult(Result.Continue<IConcreteTypeBuilder>());
    }
}
