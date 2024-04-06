namespace ClassFramework.Pipelines.Builder.Features;

public class AddBuildMethodComponentBuilder : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddBuildMethodComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<IConcreteTypeBuilder, BuilderContext> Build()
        => new AddBuildMethodComponent(_formattableStringParser);
}

public class AddBuildMethodComponent : IPipelineComponent<IConcreteTypeBuilder, BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddBuildMethodComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
    {
        context = context.IsNotNull(nameof(context));

        var returnType = context.Context.Settings.InheritFromInterfaces
            ? context.Context.SourceModel.Interfaces.FirstOrDefault(x => x.GetClassName() == $"I{context.Context.SourceModel.Name}") ?? context.Context.SourceModel.GetFullName()
            : context.Context.SourceModel.GetFullName();

        if (context.Context.Settings.EnableBuilderInheritance && context.Context.Settings.IsAbstract)
        {
            if (context.Context.Settings.IsForAbstractBuilder)
            {
                context.Model.AddMethods(new MethodBuilder()
                    .WithName("Build")
                    .WithAbstract()
                    .WithReturnTypeName(returnType));
            }
            else
            {
                context.Model.AddMethods(new MethodBuilder()
                    .WithName("Build")
                    .WithOverride()
                    .WithReturnTypeName(returnType)
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
            .WithReturnTypeName($"{GetBuilderBuildMethodReturnType(context.Context, context.Context.IsBuilderForAbstractEntity || context.Context.IsBuilderForOverrideEntity ? context.Context.SourceModel.GetFullName() : returnType)}{context.Context.SourceModel.GetGenericTypeArgumentsString()}")
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

    private static string GetName(PipelineContext<IConcreteTypeBuilder, BuilderContext> context)
        => context.Context.IsBuilderForAbstractEntity || context.Context.IsBuilderForOverrideEntity
            ? context.Context.Settings.BuildTypedMethodName
            : context.Context.Settings.BuildMethodName;

    private static string GetBuilderBuildMethodReturnType(BuilderContext context, string returnType)
        => context.IsBuilderForAbstractEntity
            ? "TEntity"
            : returnType;
}
