﻿namespace ClassFramework.Pipelines.Reflection.Components;

public class SetNameComponentBuilder(IFormattableStringParser formattableStringParser) : IReflectionComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public IPipelineComponent<ReflectionContext> Build()
        => new SetNameComponent(_formattableStringParser);
}

public class SetNameComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<ReflectionContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> Process(PipelineContext<ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var results = new ResultDictionaryBuilder<FormattableStringParserResult>()
            .Add(NamedResults.Name, () => _formattableStringParser.Parse(context.Request.Settings.NameFormatString, context.Request.FormatProvider, context.Request))
            .Add(NamedResults.Namespace, () => _formattableStringParser.Parse(context.Request.Settings.NamespaceFormatString, context.Request.FormatProvider, context.Request))
            .Build();

        var error = results.GetError();
        if (error is not null)
        {
            // Error in formattable string parsing
            return Task.FromResult<Result>(error);
        }

        context.Request.Builder
            .WithName(results[NamedResults.Name].Value!)
            .WithNamespace(context.Request.MapNamespace(results[NamedResults.Namespace].Value!));

        return Task.FromResult(Result.Continue());
    }
}
