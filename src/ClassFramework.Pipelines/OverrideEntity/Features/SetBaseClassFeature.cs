namespace ClassFramework.Pipelines.OverrideEntity.Features;

public class SetBaseClassFeatureBuilder : IOverrideEntityFeatureBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetBaseClassFeatureBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext> Build()
        => new SetBaseClassFeature(_formattableStringParser);
}

public class SetBaseClassFeature : IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public SetBaseClassFeature(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, OverrideEntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Model is IBaseClassContainerBuilder baseClassContainerBuilder)
        {
            var nameResult = _formattableStringParser.Parse(context.Context.Settings.EntityNameFormatString, context.Context.FormatProvider, context);
            if (!nameResult.IsSuccessful())
            {
                return Result.FromExistingResult<IConcreteTypeBuilder>(nameResult);
            }

            baseClassContainerBuilder.WithBaseClass($"{nameResult.Value}Base");
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>> ToBuilder()
        => new SetBaseClassFeatureBuilder(_formattableStringParser);
}
