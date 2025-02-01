namespace ClassFramework.Pipelines.Builder.Components;

public class AddBuildMethodComponent(IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper) : IPipelineComponent<BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    private readonly ICsharpExpressionDumper _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));

    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.Settings.EnableBuilderInheritance && context.Request.Settings.IsAbstract)
        {
            if (context.Request.Settings.IsForAbstractBuilder)
            {
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(context.Request.Settings.BuildMethodName)
                    .WithAbstract()
                    .WithReturnTypeName(context.Request.ReturnType));

                AddExplicitInterfaceImplementations(context);
            }
            else
            {
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(context.Request.Settings.BuildMethodName)
                    .WithOverride()
                    .WithReturnTypeName(context.Request.ReturnType)
                    .AddStringCodeStatements($"return {context.Request.Settings.BuildTypedMethodName}();"));

                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(context.Request.Settings.BuildTypedMethodName)
                    .WithAbstract()
                    .WithReturnTypeName("TEntity"));
            }

            return Task.FromResult(Result.Success());
        }

        var instanciationResult = context.CreateEntityInstanciation(_formattableStringParser, _csharpExpressionDumper, string.Empty);
        if (!instanciationResult.IsSuccessful())
        {
            return Task.FromResult<Result>(instanciationResult);
        }

        context.Request.Builder.AddMethods(new MethodBuilder()
            .WithName(GetName(context))
            .WithAbstract(context.Request.IsBuilderForAbstractEntity)
            .WithOverride(context.Request.IsBuilderForOverrideEntity)
            .WithReturnTypeName($"{GetBuilderBuildMethodReturnType(context.Request, context.Request.ReturnType)}")
            .AddStringCodeStatements(context.Request.CreatePragmaWarningDisableStatementsForBuildMethod())
            .AddStringCodeStatements
            (
                context.Request.IsBuilderForAbstractEntity
                    ? []
                    : [$"return {instanciationResult.Value};"]
            )
            .AddStringCodeStatements(context.Request.CreatePragmaWarningRestoreStatementsForBuildMethod()));

        if (context.Request.IsBuilderForAbstractEntity)
        {
            var baseClass = context.Request.Settings.BaseClass ?? context.Request.SourceModel;
            context.Request.Builder.AddMethods(new MethodBuilder()
                .WithName(context.Request.Settings.BuildMethodName)
                .WithOverride()
                .WithReturnTypeName($"{baseClass.GetFullName()}{baseClass.GetGenericTypeArgumentsString()}")
                .AddStringCodeStatements($"return {context.Request.Settings.BuildTypedMethodName}();"));
        }

        AddExplicitInterfaceImplementations(context);

        return Task.FromResult(Result.Success());
    }

    private static void AddExplicitInterfaceImplementations(PipelineContext<BuilderContext> context)
    {
        if (!context.Request.Settings.UseBuilderAbstractionsTypeConversion)
        {
            return;
        }

        foreach (var @interface in context.Request.SourceModel.Interfaces)
        {
            var typeName = context.Request.MapTypeName(@interface);
            if (typeName.GetNamespaceWithDefault().EndsWith(".Builders.Abstractions"))
            {
                // We need to add explicit implementation of the Build method of this interface
                /// Like: IVisibilityContainer IVisibilityContainerBuilder.Build() => Build();
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(context.Request.Settings.BuildMethodName)
                    .WithReturnTypeName(context.Request.MapTypeName(typeName, MetadataNames.CustomEntityInterfaceTypeName))
                    .WithExplicitInterfaceName(typeName)
                    .AddStringCodeStatements($"return {context.Request.Settings.BuildMethodName}();"));
            }
        }
    }

    private static string GetName(PipelineContext<BuilderContext> context)
        => context.Request.IsBuilderForAbstractEntity || context.Request.IsBuilderForOverrideEntity
            ? context.Request.Settings.BuildTypedMethodName
            : context.Request.Settings.BuildMethodName;

    private static string GetBuilderBuildMethodReturnType(BuilderContext context, string returnType)
        => context.IsBuilderForAbstractEntity
            ? "TEntity"
            : returnType;
}
