namespace ClassFramework.Pipelines.Builder.Components;

public class BaseClassComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(BuilderContext context, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        return (await GetBuilderBaseClassAsync(context.SourceModel, context, token)
            .ConfigureAwait(false))
            .OnSuccess(baseClassResult =>
            {
                context.Builder.WithBaseClass(baseClassResult.Value!);
            });
    }

    private async Task<Result<GenericFormattableString>> GetBuilderBaseClassAsync(IType instance, BuilderContext context, CancellationToken token)
    {
        var nameResult = await _evaluator.EvaluateInterpolatedStringAsync(context.Settings.BuilderNameFormatString, context.FormatProvider, context, token).ConfigureAwait(false);
        if (!nameResult.IsSuccessful())
        {
            return nameResult;
        }

        var genericTypeArgumentsString = instance.GetGenericTypeArgumentsString();

        var isNotForAbstractBuilder = context.Settings.EnableInheritance
            && context.Settings.EnableBuilderInheritance
            && context.Settings.BaseClass is null
            && !context.Settings.IsForAbstractBuilder;

        var isAbstract = context.Settings.EnableInheritance
            && context.Settings.EnableBuilderInheritance
            && context.Settings.BaseClass is not null
            && !context.Settings.IsForAbstractBuilder
            && context.Settings.IsAbstract;

        if (isNotForAbstractBuilder || isAbstract)
        {
            return Result.Success<GenericFormattableString>($"{nameResult.Value}{genericTypeArgumentsString}");
        }

        if (context.Settings.EnableInheritance
            && context.Settings.EnableBuilderInheritance
            && context.Settings.BaseClass is not null
            && !context.Settings.IsForAbstractBuilder) // note that originally, this was only enabled when RemoveDuplicateWithMethods was true. But I don't know why you don't want this... The generics ensure that we don't have to duplicate them, right?
        {
            var inheritanceNameResult = await _evaluator.EvaluateInterpolatedStringAsync
            (
                context.Settings.BuilderNameFormatString,
                context.FormatProvider,
                new BuilderContext(context.Settings.BaseClass!, context.Settings, context.FormatProvider, CancellationToken.None),
                token
            ).ConfigureAwait(false);

            if (!inheritanceNameResult.IsSuccessful())
            {
                return inheritanceNameResult;
            }

            return Result.Success<GenericFormattableString>($"{context.Settings.BaseClassBuilderNameSpace.AppendWhenNotNullOrEmpty(".")}{inheritanceNameResult.Value}<{nameResult.Value}{genericTypeArgumentsString}, {instance.GetFullName()}{genericTypeArgumentsString}>");
        }

        return await instance.GetCustomValueForInheritedClass
        (
            context.Settings.EnableInheritance,
            async baseClassContainer =>
            {
                var baseClassResult = await GetBaseClassNameAsync(context, baseClassContainer, token).ConfigureAwait(false);
                if (!baseClassResult.IsSuccessful())
                {
                    return baseClassResult;
                }

                context.Builder.Interfaces
                    .Where(x => x.WithoutGenerics() == typeof(IBuilder<object>).WithoutGenerics())
                    .ToList()
                    .ForEach(x => context.Builder.Interfaces.Remove(x));

                return Result.Success<GenericFormattableString>(context.Settings.EnableBuilderInheritance
                    ? $"{baseClassResult.Value}{genericTypeArgumentsString}"
                    : $"{baseClassResult.Value}<{nameResult.Value}{genericTypeArgumentsString}, {instance.GetFullName()}{genericTypeArgumentsString}>");
            }
        ).ConfigureAwait(false);
    }

    private async Task<Result<GenericFormattableString>> GetBaseClassNameAsync(BuilderContext context, IBaseClassContainer baseClassContainer, CancellationToken token)
    {
        var customValue = context.GetMappingMetadata(baseClassContainer.BaseClass).GetStringValue(MetadataNames.CustomBuilderBaseClassTypeName);
        if (!string.IsNullOrEmpty(customValue))
        {
            return Result.Success(new GenericFormattableString(customValue));
        }

        return await _evaluator.EvaluateInterpolatedStringAsync(context.Settings.BuilderNameFormatString, context.FormatProvider, new BuilderContext(CreateTypeBase(context.MapTypeName(baseClassContainer.BaseClass!)), context.Settings, context.FormatProvider, token), token).ConfigureAwait(false);
    }

    private static TypeBase CreateTypeBase(string baseClass)
        => new ClassBuilder()
            .WithNamespace(baseClass.GetNamespaceWithDefault())
            .WithName(baseClass.GetClassName())
            .Build();
}
