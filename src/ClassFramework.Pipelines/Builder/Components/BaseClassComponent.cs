namespace ClassFramework.Pipelines.Builder.Components;

public class BaseClassComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var baseClassResult = await GetBuilderBaseClass(context.Request.SourceModel, context, token).ConfigureAwait(false);
        if (!baseClassResult.IsSuccessful())
        {
            return baseClassResult;
        }

        context.Request.Builder.WithBaseClass(baseClassResult.Value!);

        return Result.Success();
    }

    private async Task<Result<GenericFormattableString>> GetBuilderBaseClass(IType instance, PipelineContext<BuilderContext> context, CancellationToken token)
    {
        var genericTypeArgumentsString = instance.GetGenericTypeArgumentsString();

        var isNotForAbstractBuilder = context.Request.Settings.EnableInheritance
            && context.Request.Settings.EnableBuilderInheritance
            && context.Request.Settings.BaseClass is null
            && !context.Request.Settings.IsForAbstractBuilder;

        var isAbstract = context.Request.Settings.EnableInheritance
            && context.Request.Settings.EnableBuilderInheritance
            && context.Request.Settings.BaseClass is not null
            && !context.Request.Settings.IsForAbstractBuilder
            && context.Request.Settings.IsAbstract;

        var nameResult = await _evaluator.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, context.Request, token).ConfigureAwait(false);

        if (!nameResult.IsSuccessful())
        {
            return nameResult;
        }

        if (isNotForAbstractBuilder || isAbstract)
        {
            return Result.Success<GenericFormattableString>($"{nameResult.Value}{genericTypeArgumentsString}");
        }

        if (context.Request.Settings.EnableInheritance
            && context.Request.Settings.EnableBuilderInheritance
            && context.Request.Settings.BaseClass is not null
            && !context.Request.Settings.IsForAbstractBuilder) // note that originally, this was only enabled when RemoveDuplicateWithMethods was true. But I don't know why you don't want this... The generics ensure that we don't have to duplicate them, right?
        {
            var inheritanceNameResult = await _evaluator.Parse
            (
                context.Request.Settings.BuilderNameFormatString,
                context.Request.FormatProvider,
                new BuilderContext(context.Request.Settings.BaseClass!, context.Request.Settings, context.Request.FormatProvider),
                token
            ).ConfigureAwait(false);

            if (!inheritanceNameResult.IsSuccessful())
            {
                return inheritanceNameResult;
            }

            return Result.Success<GenericFormattableString>($"{context.Request.Settings.BaseClassBuilderNameSpace.AppendWhenNotNullOrEmpty(".")}{inheritanceNameResult.Value}<{nameResult.Value}{genericTypeArgumentsString}, {instance.GetFullName()}{genericTypeArgumentsString}>");
        }

        return await instance.GetCustomValueForInheritedClass
        (
            context.Request.Settings.EnableInheritance,
            async baseClassContainer =>
            {
                var baseClassResult = await GetBaseClassName(context, baseClassContainer, token).ConfigureAwait(false);
                if (!baseClassResult.IsSuccessful())
                {
                    return baseClassResult;
                }

                return Result.Success<GenericFormattableString>(context.Request.Settings.EnableBuilderInheritance
                    ? $"{baseClassResult.Value}{genericTypeArgumentsString}"
                    : $"{baseClassResult.Value}<{nameResult.Value}{genericTypeArgumentsString}, {instance.GetFullName()}{genericTypeArgumentsString}>");
            }
        ).ConfigureAwait(false);
    }

    private Task<Result<GenericFormattableString>> GetBaseClassName(PipelineContext<BuilderContext> context, IBaseClassContainer baseClassContainer, CancellationToken token)
        => _evaluator.Parse(context.Request.Settings.BuilderNameFormatString, context.Request.FormatProvider, new BuilderContext(CreateTypeBase(context.Request.MapTypeName(baseClassContainer.BaseClass!, string.Empty)), context.Request.Settings, context.Request.FormatProvider), token);

    private static TypeBase CreateTypeBase(string baseClass)
        => new ClassBuilder()
            .WithNamespace(baseClass.GetNamespaceWithDefault())
            .WithName(baseClass.GetClassName())
            .Build();
}
