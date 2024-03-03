namespace ClassFramework.Pipelines.Builder.Features;

public class AddBuildMethodFeatureBuilder : IBuilderFeatureBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddBuildMethodFeatureBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineFeature<IConcreteTypeBuilder, BuilderContext> Build()
        => new AddBuildMethodFeature(_formattableStringParser);
}

public class AddBuildMethodFeature : IPipelineFeature<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddBuildMethodFeature(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Context.Settings.EnableBuilderInheritance && context.Context.Settings.IsAbstract)
        {
            if (context.Context.Settings.IsForAbstractBuilder)
            {
                context.Model.AddMethods(new MethodBuilder()
                    .WithName("Build")
                    .WithAbstract()
                    .WithReturnTypeName(context.Context.SourceModel.GetFullName()));
            }
            else
            {
                context.Model.AddMethods(new MethodBuilder()
                    .WithName("Build")
                    .WithOverride()
                    .WithReturnTypeName(context.Context.SourceModel.GetFullName())
                    .AddStringCodeStatements("return BuildTyped();"));

                context.Model.AddMethods(new MethodBuilder()
                    .WithName("BuildTyped")
                    .WithAbstract()
                    .WithReturnTypeName("TEntity"));
            }

            return Result.Continue<IConcreteTypeBuilder>();
        }

        var instanciationResult = context.CreateEntityInstanciation(_formattableStringParser, string.Empty);
        if (!instanciationResult.IsSuccessful())
        {
            return Result.FromExistingResult<IConcreteTypeBuilder>(instanciationResult);
        }

        context.Model.AddMethods(new MethodBuilder()
            .WithName(GetName(context))
            .WithAbstract(context.Context.IsBuilderForAbstractEntity)
            .WithOverride(context.Context.IsBuilderForOverrideEntity)
            .WithReturnTypeName($"{GetBuilderBuildMethodReturnType(context.Context)}{context.Context.SourceModel.GetGenericTypeArgumentsString()}")
            .AddStringCodeStatements(context.Context.CreatePragmaWarningDisableStatementsForBuildMethod())
            .AddStringCodeStatements
            (
                context.Context.IsBuilderForAbstractEntity
                    ? Array.Empty<string>()
                    : [$"return {instanciationResult.Value};"]
            )
            .AddStringCodeStatements(context.Context.CreatePragmaWarningRestoreStatementsForBuildMethod()));

        if (context.Context.IsBuilderForAbstractEntity)
        {
            var baseClass = context.Context.Settings.BaseClass ?? context.Context.SourceModel;
            context.Model.AddMethods(new MethodBuilder()
                .WithName(context.Context.Settings.BuildMethodName)
                .WithOverride()
                .WithReturnTypeName($"{baseClass.GetFullName()}{baseClass.GetGenericTypeArgumentsString()}")
                .AddStringCodeStatements($"return {context.Context.Settings.BuildTypedMethodName}();"));
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }

    public IBuilder<IPipelineFeature<IConcreteTypeBuilder, BuilderContext>> ToBuilder()
        => new AddBuildMethodFeatureBuilder(_formattableStringParser);

    private static string GetName(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
        => context.Context.IsBuilderForAbstractEntity || context.Context.IsBuilderForOverrideEntity
            ? context.Context.Settings.BuildTypedMethodName
            : context.Context.Settings.BuildMethodName;

    private static string GetBuilderBuildMethodReturnType(BuilderContext context)
        => context.IsBuilderForAbstractEntity
            ? "TEntity"
            : context.SourceModel.GetFullName();
}
