﻿namespace ClassFramework.Pipelines.Interface.Components;

public class SetNameComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<InterfaceContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var results = new ResultDictionaryBuilder<GenericFormattableString>()
            .Add(NamedResults.Name, () => _formattableStringParser.Parse(context.Request.Settings.NameFormatString, context.Request.FormatProvider, context.Request))
            .Add(NamedResults.Namespace, () => context.Request.GetMappingMetadata(context.Request.SourceModel.GetFullName()).GetGenericFormattableString(MetadataNames.CustomEntityNamespace, () => _formattableStringParser.Parse(context.Request.Settings.NamespaceFormatString, context.Request.FormatProvider, context.Request)))
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

        return Task.FromResult(Result.Success());
    }
}
