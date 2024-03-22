namespace ClassFramework.Pipelines.OverrideEntity.Features;

public class AddConstructorFeatureBuilder : IOverrideEntityFeatureBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddConstructorFeatureBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext> Build()
        => new AddConstructorFeature(_formattableStringParser);
}

public class AddConstructorFeature : IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddConstructorFeature(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, OverrideEntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        var ctorResult = CreateOverrideEntityConstructor(context);
        if (!ctorResult.IsSuccessful())
        {
            return Result.FromExistingResult<IConcreteTypeBuilder>(ctorResult);
        }

        context.Model.AddConstructors(ctorResult.Value!);

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, OverrideEntityContext>> ToBuilder()
        => new AddConstructorFeatureBuilder(_formattableStringParser);

    private Result<ConstructorBuilder> CreateOverrideEntityConstructor(PipelineContext<IConcreteTypeBuilder, OverrideEntityContext> context)
    {
        var parameters = string.Join(", ", context.Context.SourceModel.Properties.Select(x => x.Name.ToPascalCase(context.Context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()));

        return Result.Success(new ConstructorBuilder()
            .WithProtected(context.Context.Settings.EnableInheritance && context.Context.Settings.IsAbstract)
            .WithChainCall($"base({parameters})")
            .AddParameters(context.Context.SourceModel.Properties.CreateImmutableClassCtorParameters(context.Context.FormatProvider, context.Context.Settings, n => context.Context.MapTypeName(n, MetadataNames.CustomEntityInterfaceTypeName)))
            .AddStringCodeStatements(context.Context.CreateEntityValidationCode(context.Context.SourceModel, false))
            );
    }
}
