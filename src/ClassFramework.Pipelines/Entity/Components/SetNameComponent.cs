﻿namespace ClassFramework.Pipelines.Entity.Components;

public class SetNameComponentBuilder(IFormattableStringParser formattableStringParser) : IEntityComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public IPipelineComponent<EntityContext> Build()
        => new SetNameComponent(_formattableStringParser);
}

public class SetNameComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<EntityContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> Process(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var resultSetBuilder = new NamedResultSetBuilder<FormattableStringParserResult>();
        resultSetBuilder.Add(NamedResults.Name, () => _formattableStringParser.Parse(context.Request.Settings.EntityNameFormatString, context.Request.FormatProvider, context.Request));
        resultSetBuilder.Add(NamedResults.Namespace, () => context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetFormattableStringParserResult(MetadataNames.CustomEntityNamespace, () => _formattableStringParser.Parse(context.Request.Settings.EntityNamespaceFormatString, context.Request.FormatProvider, context.Request)));
        var results = resultSetBuilder.Build();

        var error = Array.Find(results, x => !x.Result.IsSuccessful());
        if (error is not null)
        {
            // Error in formattable string parsing
            return Task.FromResult<Result>(error.Result);
        }

        context.Request.Builder
            .WithName(results.First(x => x.Name == NamedResults.Name).Result.Value!)
            .WithNamespace(context.Request.MapNamespace(results.First(x => x.Name == NamedResults.Namespace).Result.Value!));

        return Task.FromResult(Result.Continue());
    }
}
