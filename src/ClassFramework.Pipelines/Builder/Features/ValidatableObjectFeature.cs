namespace ClassFramework.Pipelines.Builder.Features;

public class ValidatableObjectFeatureBuilder : IBuilderFeatureBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public ValidatableObjectFeatureBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineFeature<IConcreteTypeBuilder, BuilderContext> Build()
        => new ValidatableObjectFeature(_formattableStringParser);
}

public class ValidatableObjectFeature : IPipelineFeature<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public ValidatableObjectFeature(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Context.IsBuilderForAbstractEntity || context.Context.Settings.OriginalValidateArguments != ArgumentValidationType.Shared)
        {
            return Result.Continue<IConcreteTypeBuilder>();
        }

        var instanciationResult = context.CreateEntityInstanciation(_formattableStringParser, "Base");
        if (!instanciationResult.IsSuccessful())
        {
            return Result.FromExistingResult<IConcreteTypeBuilder>(instanciationResult);
        }

        context.Model
            .AddInterfaces(typeof(IValidatableObject))
            .AddMethods(new MethodBuilder()
                .WithName(nameof(IValidatableObject.Validate))
                .WithReturnType(typeof(IEnumerable<ValidationResult>))
                .AddParameter("validationContext", typeof(ValidationContext))
                .AddStringCodeStatements(context.Context.CreatePragmaWarningDisableStatements())
                .AddStringCodeStatements($"var instance = {instanciationResult.Value};")
                .AddStringCodeStatements(context.Context.CreatePragmaWarningRestoreStatements())
                .AddStringCodeStatements
                (
                    context.Context.SourceModel.Metadata.GetStringValues(MetadataNames.CustomBuilderValidationCode).WhenEmpty(() =>
                    new[]
                    {
                        $"var results = new {typeof(List<>).ReplaceGenericTypeName(typeof(ValidationResult))}();",
                        $"{typeof(Validator).FullName}.{nameof(Validator.TryValidateObject)}(instance, new {typeof(ValidationContext).FullName}(instance), results, true);",
                        "return results;"
                    })
                )
            );

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, BuilderContext>> ToBuilder()
        => new ValidatableObjectFeatureBuilder(_formattableStringParser);
}
