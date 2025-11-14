namespace ClassFramework.Pipelines.Entity.Components;

public class AddFullConstructorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateEntityCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        if (!command.Settings.AddFullConstructor)
        {
            return Result.Continue();
        }

        return (await CreateEntityConstructorAsync(command, token)
            .ConfigureAwait(false))
            .OnSuccess(ctorResult =>
            {
                response.AddConstructors(ctorResult.Value!);

                if (command.Settings.AddValidationCode() == ArgumentValidationType.CustomValidationCode)
                {
                    response.AddMethods(new MethodBuilder()
                        .WithName("Validate")
                        .WithPartial()
                        .WithVisibility(Visibility.Private));
                }
            });
    }

    private async Task<Result<ConstructorBuilder>> CreateEntityConstructorAsync(GenerateEntityCommand command, CancellationToken token)
    {
        var initializationResults = new List<Result<GenericFormattableString>>();

        foreach (var property in command.GetSourceProperties())
        {
            var result = await _evaluator.EvaluateInterpolatedStringAsync("this.{property.EntityMemberName} = {property.InitializationExpression};", command.FormatProvider, new ParentChildContext<GenerateEntityCommand, Property>(command, property, command.Settings), token).ConfigureAwait(false);

            initializationResults.Add(result);
            if (!result.IsSuccessful())
            {
                break;
            }
        }

        var error = initializationResults.Find(x => !x.IsSuccessful());
        if (error is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(error);
        }

        return Result.Success(new ConstructorBuilder()
            .WithProtected(command.Settings.EnableInheritance && command.Settings.IsAbstract)
            .AddParameters(command.SourceModel.Properties.CreateImmutableClassCtorParameters(command.FormatProvider, n => command.MapTypeName(n, MetadataNames.CustomEntityInterfaceTypeName)))
            .AddCodeStatements
            (
                command.GetSourceProperties()
                    .Where(property => command.Settings.AddNullChecks && command.Settings.AddValidationCode() == ArgumentValidationType.None && command.GetMappingMetadata(property.TypeName).GetValue(MetadataNames.EntityNullCheck, () => !property.IsNullable && !property.IsValueType))
                    .Select(property => command.CreateArgumentNullException(property.Name.ToCamelCase(command.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()))
            )
            .AddCodeStatements(initializationResults.Select(x => x.Value!.ToString()))
            .AddCodeStatements(command.CreateEntityValidationCode())
            .WithChainCall(await command.CreateEntityChainCallAsync().ConfigureAwait(false)));
    }
}
