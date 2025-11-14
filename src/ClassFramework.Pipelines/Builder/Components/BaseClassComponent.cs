namespace ClassFramework.Pipelines.Builder.Components;

public class BaseClassComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        return (await GetBuilderBaseClassAsync(command.SourceModel, command, response, token)
            .ConfigureAwait(false))
            .OnSuccess(baseClassResult =>
            {
                response.WithBaseClass(baseClassResult.Value!);
            });
    }

    private async Task<Result<GenericFormattableString>> GetBuilderBaseClassAsync(IType instance, GenerateBuilderCommand command, ClassBuilder response, CancellationToken token)
    {
        var nameResult = await _evaluator.EvaluateInterpolatedStringAsync(command.Settings.BuilderNameFormatString, command.FormatProvider, command, token).ConfigureAwait(false);
        if (!nameResult.IsSuccessful())
        {
            return nameResult;
        }

        var genericTypeArgumentsString = instance.GetGenericTypeArgumentsString();

        var isNotForAbstractBuilder = command.Settings.EnableInheritance
            && command.Settings.EnableBuilderInheritance
            && command.Settings.BaseClass is null
            && !command.Settings.IsForAbstractBuilder;

        var isAbstract = command.Settings.EnableInheritance
            && command.Settings.EnableBuilderInheritance
            && command.Settings.BaseClass is not null
            && !command.Settings.IsForAbstractBuilder
            && command.Settings.IsAbstract;

        if (isNotForAbstractBuilder || isAbstract)
        {
            return Result.Success<GenericFormattableString>($"{nameResult.Value}{genericTypeArgumentsString}");
        }

        if (command.Settings.EnableInheritance
            && command.Settings.EnableBuilderInheritance
            && command.Settings.BaseClass is not null
            && !command.Settings.IsForAbstractBuilder) // note that originally, this was only enabled when RemoveDuplicateWithMethods was true. But I don't know why you don't want this... The generics ensure that we don't have to duplicate them, right?
        {
            var inheritanceNameResult = await _evaluator.EvaluateInterpolatedStringAsync
            (
                command.Settings.BuilderNameFormatString,
                command.FormatProvider,
                new GenerateBuilderCommand(command.Settings.BaseClass!, command.Settings, command.FormatProvider),
                token
            ).ConfigureAwait(false);

            if (!inheritanceNameResult.IsSuccessful())
            {
                return inheritanceNameResult;
            }

            return Result.Success<GenericFormattableString>($"{command.Settings.BaseClassBuilderNameSpace.AppendWhenNotNullOrEmpty(".")}{inheritanceNameResult.Value}<{nameResult.Value}{genericTypeArgumentsString}, {instance.GetFullName()}{genericTypeArgumentsString}>");
        }

        return await instance.GetCustomValueForInheritedClassAsync
        (
            command.Settings.EnableInheritance,
            async baseClassContainer =>
            {
                var baseClassResult = await GetBaseClassNameAsync(command, baseClassContainer, token).ConfigureAwait(false);
                if (!baseClassResult.IsSuccessful())
                {
                    return baseClassResult;
                }

                response.Interfaces
                    .Where(x => x.WithoutGenerics() == typeof(IBuilder<object>).WithoutGenerics())
                    .ToList()
                    .ForEach(x => response.Interfaces.Remove(x));

                return Result.Success<GenericFormattableString>(command.Settings.EnableBuilderInheritance
                    ? $"{baseClassResult.Value}{genericTypeArgumentsString}"
                    : $"{baseClassResult.Value}<{nameResult.Value}{genericTypeArgumentsString}, {instance.GetFullName()}{genericTypeArgumentsString}>");
            }
        ).ConfigureAwait(false);
    }

    private async Task<Result<GenericFormattableString>> GetBaseClassNameAsync(GenerateBuilderCommand command, IBaseClassContainer baseClassContainer, CancellationToken token)
    {
        var customValue = command.GetMappingMetadata(baseClassContainer.BaseClass).GetStringValue(MetadataNames.CustomBuilderBaseClassTypeName);
        if (!string.IsNullOrEmpty(customValue))
        {
            return Result.Success(new GenericFormattableString(customValue));
        }

        return await _evaluator.EvaluateInterpolatedStringAsync(command.Settings.BuilderNameFormatString, command.FormatProvider, new GenerateBuilderCommand(CreateTypeBase(command.MapTypeName(baseClassContainer.BaseClass!)), command.Settings, command.FormatProvider), token).ConfigureAwait(false);
    }

    private static TypeBase CreateTypeBase(string baseClass)
        => new ClassBuilder()
            .WithNamespace(baseClass.GetNamespaceWithDefault())
            .WithName(baseClass.GetClassName())
            .Build();
}
