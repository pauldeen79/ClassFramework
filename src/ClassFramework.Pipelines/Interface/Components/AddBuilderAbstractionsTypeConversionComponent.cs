namespace ClassFramework.Pipelines.Interface.Components;

public class AddBuilderAbstractionsTypeConversionComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<InterfaceContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.UseBuilderAbstractionsTypeConversion)
        {
            return Task.FromResult(Result.Success());
        }

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

        // TODO: Make a check that's more safe. Or we have to inject something into the settings...
        if (context.Request.SourceModel.Namespace.EndsWith(".Builders", StringComparison.Ordinal))
        {
            // Builder
            if (context.Request.Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(results[NamedResults.Namespace].Value!.ToString()))
            {
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(context.Request.Settings.BuildMethodName)
                    .WithExplicitInterfaceName(context.Request.SourceModel.GetFullName()) //TODO: How do we get the abstraction builder interface name?
                    .WithReturnTypeName(context.Request.SourceModel.GetFullName())
                    .AddStringCodeStatements($"return {context.Request.Settings.BuildMethodName}();"));
            }

            var interfaces = context.Request.SourceModel.Interfaces.Where(x => context.Request.Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(x.GetNamespaceWithDefault()));

            context.Request.Builder.AddMethods(interfaces.Select(x => new MethodBuilder()
                .WithName(context.Request.Settings.BuildMethodName)
                .WithExplicitInterfaceName(x) //TODO: How do we get the abstraction builder interface name?
                .WithReturnTypeName(x)
                .AddStringCodeStatements($"return {context.Request.Settings.BuildMethodName}();")));
        }
        else
        {
            // Entity
            if (context.Request.Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(results[NamedResults.Namespace].Value!.ToString()))
            {
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(context.Request.Settings.ToBuilderFormatString)
                    .WithExplicitInterfaceName(context.Request.SourceModel.GetFullName()) //TODO: How do we get the abstraction builder interface name?
                    .WithReturnTypeName(context.Request.SourceModel.GetFullName())
                    .AddStringCodeStatements($"return {context.Request.Settings.ToBuilderFormatString}();"));
            }

            var interfaces = context.Request.SourceModel.Interfaces.Where(x => context.Request.Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(x.GetNamespaceWithDefault()));

            context.Request.Builder.AddMethods(interfaces.Select(x => new MethodBuilder()
                .WithName(context.Request.Settings.ToBuilderFormatString)
                .WithExplicitInterfaceName(x) //TODO: How do we get the abstraction builder interface name?
                .WithReturnTypeName(x)
                .AddStringCodeStatements($"return {context.Request.Settings.ToBuilderFormatString}();")));
        }

        return Task.FromResult(Result.Success());
    }
}
