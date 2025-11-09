namespace ClassFramework.Pipelines.Entity.Components;

public class AddFullConstructorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateEntityCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        if (!context.Settings.AddFullConstructor)
        {
            return Result.Continue();
        }

        return (await CreateEntityConstructorAsync(context, token)
            .ConfigureAwait(false))
            .OnSuccess(ctorResult =>
            {
                response.AddConstructors(ctorResult.Value!);

                if (context.Settings.AddValidationCode() == ArgumentValidationType.CustomValidationCode)
                {
                    response.AddMethods(new MethodBuilder()
                        .WithName("Validate")
                        .WithPartial()
                        .WithVisibility(Visibility.Private));
                }
            });
    }

    private async Task<Result<ConstructorBuilder>> CreateEntityConstructorAsync(GenerateEntityCommand context, CancellationToken token)
    {
        var initializationResults = new List<Result<GenericFormattableString>>();

        foreach (var property in context.GetSourceProperties())
        {
            var result = await _evaluator.EvaluateInterpolatedStringAsync("this.{property.EntityMemberName} = {property.InitializationExpression};", context.FormatProvider, new ParentChildContext<GenerateEntityCommand, Property>(context, property, context.Settings), token).ConfigureAwait(false);

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
            .WithProtected(context.Settings.EnableInheritance && context.Settings.IsAbstract)
            .AddParameters(context.SourceModel.Properties.CreateImmutableClassCtorParameters(context.FormatProvider, n => context.MapTypeName(n, MetadataNames.CustomEntityInterfaceTypeName)))
            .AddCodeStatements
            (
                context.GetSourceProperties()
                    .Where(property => context.Settings.AddNullChecks && context.Settings.AddValidationCode() == ArgumentValidationType.None && context.GetMappingMetadata(property.TypeName).GetValue(MetadataNames.EntityNullCheck, () => !property.IsNullable && !property.IsValueType))
                    .Select(property => context.CreateArgumentNullException(property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()))
            )
            .AddCodeStatements(initializationResults.Select(x => x.Value!.ToString()))
            .AddCodeStatements(context.CreateEntityValidationCode())
            .WithChainCall(await context.CreateEntityChainCallAsync().ConfigureAwait(false)));
    }
}
