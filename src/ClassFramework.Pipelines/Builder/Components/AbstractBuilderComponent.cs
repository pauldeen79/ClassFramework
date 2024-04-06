namespace ClassFramework.Pipelines.Builder.Features;

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

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Context.IsBuilderForAbstractEntity /*&& context.Context.IsAbstractBuilder*/)
        {
            var nameResult = _formattableStringParser.Parse(context.Context.Settings.BuilderNameFormatString, context.Context.FormatProvider, context);
            if (!nameResult.IsSuccessful())
            {
                return Result.FromExistingResult<IConcreteTypeBuilder>(nameResult);
            }

            if (context.Model is not ClassBuilder classBuilder)
            {
                return Result.Invalid<IConcreteTypeBuilder>($"You can only create abstract classes. The type of model ({context.Model.GetType().FullName}) is not a ClassBuilder");
            }

            classBuilder.WithAbstract();

            if (!context.Context.Settings.IsForAbstractBuilder)
            {
                classBuilder
                    .AddGenericTypeArguments("TBuilder", "TEntity")
                    .AddGenericTypeArgumentConstraints($"where TEntity : {context.Context.SourceModel.GetFullName()}")
                    .AddGenericTypeArgumentConstraints($"where TBuilder : {nameResult.Value}<TBuilder, TEntity>")
                    .WithAbstract();
            }
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }
}
