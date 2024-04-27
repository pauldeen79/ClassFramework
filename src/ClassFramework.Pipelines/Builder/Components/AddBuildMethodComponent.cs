namespace ClassFramework.Pipelines.Builder.Components;

public class AddBuildMethodComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;
    private readonly ICsharpExpressionDumper _csharpExpressionDumper;

    public AddBuildMethodComponentBuilder(IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
        _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));
    }

    public IPipelineComponent<BuilderContext, IConcreteTypeBuilder> Build()
        => new AddBuildMethodComponent(_formattableStringParser, _csharpExpressionDumper);
}

public class AddBuildMethodComponent : IPipelineComponent<BuilderContext, IConcreteTypeBuilder>
{
    private readonly IFormattableStringParser _formattableStringParser;
    private readonly ICsharpExpressionDumper _csharpExpressionDumper;

    public AddBuildMethodComponent(IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
        _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));
    }

    public Task<Result> Process(PipelineContext<BuilderContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var returnType = context.Request.Settings.InheritFromInterfaces
            ? context.Request.SourceModel.Interfaces.FirstOrDefault(x => x.GetClassName() == $"I{context.Request.SourceModel.Name}") ?? context.Request.SourceModel.GetFullName()
            : context.Request.SourceModel.GetFullName();

        if (context.Request.Settings.EnableBuilderInheritance && context.Request.Settings.IsAbstract)
        {
            if (context.Request.Settings.IsForAbstractBuilder)
            {
                context.Response.AddMethods(new MethodBuilder()
                    .WithName("Build")
                    .WithAbstract()
                    .WithReturnTypeName(returnType));
            }
            else
            {
                context.Response.AddMethods(new MethodBuilder()
                    .WithName("Build")
                    .WithOverride()
                    .WithReturnTypeName(returnType)
                    .AddStringCodeStatements("return BuildTyped();"));

                context.Response.AddMethods(new MethodBuilder()
                    .WithName("BuildTyped")
                    .WithAbstract()
                    .WithReturnTypeName("TEntity"));
            }

            return Task.FromResult(Result.Continue());
        }

        var instanciationResult = context.CreateEntityInstanciation(_formattableStringParser, _csharpExpressionDumper, string.Empty);
        if (!instanciationResult.IsSuccessful())
        {
            return Task.FromResult<Result>(instanciationResult);
        }

        context.Response.AddMethods(new MethodBuilder()
            .WithName(GetName(context))
            .WithAbstract(context.Request.IsBuilderForAbstractEntity)
            .WithOverride(context.Request.IsBuilderForOverrideEntity)
            .WithReturnTypeName($"{GetBuilderBuildMethodReturnType(context.Request, context.Request.IsBuilderForAbstractEntity || context.Request.IsBuilderForOverrideEntity ? context.Request.SourceModel.GetFullName() : returnType)}{context.Request.SourceModel.GetGenericTypeArgumentsString()}")
            .AddStringCodeStatements(context.Request.CreatePragmaWarningDisableStatementsForBuildMethod())
            .AddStringCodeStatements
            (
                context.Request.IsBuilderForAbstractEntity
                    ? Array.Empty<string>()
                    : [$"return {instanciationResult.Value};"]
            )
            .AddStringCodeStatements(context.Request.CreatePragmaWarningRestoreStatementsForBuildMethod()));

        if (context.Request.IsBuilderForAbstractEntity)
        {
            var baseClass = context.Request.Settings.BaseClass ?? context.Request.SourceModel;
            context.Response.AddMethods(new MethodBuilder()
                .WithName(context.Request.Settings.BuildMethodName)
                .WithOverride()
                .WithReturnTypeName($"{baseClass.GetFullName()}{baseClass.GetGenericTypeArgumentsString()}")
                .AddStringCodeStatements($"return {context.Request.Settings.BuildTypedMethodName}();"));
        }

        return Task.FromResult(Result.Continue());
    }

    private static string GetName(PipelineContext<BuilderContext, IConcreteTypeBuilder> context)
        => context.Request.IsBuilderForAbstractEntity || context.Request.IsBuilderForOverrideEntity
            ? context.Request.Settings.BuildTypedMethodName
            : context.Request.Settings.BuildMethodName;

    private static string GetBuilderBuildMethodReturnType(BuilderContext context, string returnType)
        => context.IsBuilderForAbstractEntity
            ? "TEntity"
            : returnType;
}
