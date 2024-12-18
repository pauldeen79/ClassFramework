﻿namespace ClassFramework.Pipelines.Builder.Components;

public class AddBuildMethodComponentBuilder(IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper) : IBuilderComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    private readonly ICsharpExpressionDumper _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));

    public IPipelineComponent<BuilderContext> Build()
        => new AddBuildMethodComponent(_formattableStringParser, _csharpExpressionDumper);
}

public class AddBuildMethodComponent(IFormattableStringParser formattableStringParser, ICsharpExpressionDumper csharpExpressionDumper) : IPipelineComponent<BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    private readonly ICsharpExpressionDumper _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));

    public Task<Result> Process(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var returnType = context.Request.Settings.InheritFromInterfaces
            ? context.Request.SourceModel.Interfaces.FirstOrDefault(x => x.GetClassName() == $"I{context.Request.SourceModel.Name}") ?? context.Request.SourceModel.GetFullName()
            : context.Request.SourceModel.GetFullName();

        if (context.Request.Settings.EnableBuilderInheritance && context.Request.Settings.IsAbstract)
        {
            if (context.Request.Settings.IsForAbstractBuilder)
            {
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName("Build")
                    .WithAbstract()
                    .WithReturnTypeName(returnType));
            }
            else
            {
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName("Build")
                    .WithOverride()
                    .WithReturnTypeName(returnType)
                    .AddStringCodeStatements("return BuildTyped();"));

                context.Request.Builder.AddMethods(new MethodBuilder()
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

        context.Request.Builder.AddMethods(new MethodBuilder()
            .WithName(GetName(context))
            .WithAbstract(context.Request.IsBuilderForAbstractEntity)
            .WithOverride(context.Request.IsBuilderForOverrideEntity)
            .WithReturnTypeName($"{GetBuilderBuildMethodReturnType(context.Request, context.Request.IsBuilderForAbstractEntity || context.Request.IsBuilderForOverrideEntity ? context.Request.SourceModel.GetFullName() : returnType)}{context.Request.SourceModel.GetGenericTypeArgumentsString()}")
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

        return Task.FromResult(Result.Continue());
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
