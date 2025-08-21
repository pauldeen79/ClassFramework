namespace ClassFramework.Pipelines.Entity.Components;

public class AddFullConstructorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AddFullConstructor)
        {
            return Result.Success();
        }

        return (await CreateEntityConstructor(context, token)
            .ConfigureAwait(false))
            .OnSuccess(ctorResult =>
            {
                context.Request.Builder.AddConstructors(ctorResult.Value!);

                if (context.Request.Settings.AddValidationCode() == ArgumentValidationType.CustomValidationCode)
                {
                    context.Request.Builder.AddMethods(new MethodBuilder()
                        .WithName("Validate")
                        .WithPartial()
                        .WithVisibility(Visibility.Private));
                }
            });
    }

    private async Task<Result<ConstructorBuilder>> CreateEntityConstructor(PipelineContext<EntityContext> context, CancellationToken token)
    {
        var initializationResults = new List<Result<GenericFormattableString>>();

        foreach (var property in context.Request.GetSourceProperties())
        {
            var result = await _evaluator.EvaluateInterpolatedStringAsync("this.{property.EntityMemberName} = {property.InitializationExpression};", context.Request.FormatProvider, new ParentChildContext<PipelineContext<EntityContext>, Property>(context, property, context.Request.Settings), token).ConfigureAwait(false);

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
            .WithProtected(context.Request.Settings.EnableInheritance && context.Request.Settings.IsAbstract)
            .AddParameters(context.Request.SourceModel.Properties.CreateImmutableClassCtorParameters(context.Request.FormatProvider, n => context.Request.MapTypeName(n, MetadataNames.CustomEntityInterfaceTypeName)))
            .AddCodeStatements
            (
                context.Request.GetSourceProperties()
                    .Where(property => context.Request.Settings.AddNullChecks && context.Request.Settings.AddValidationCode() == ArgumentValidationType.None && context.Request.GetMappingMetadata(property.TypeName).GetValue(MetadataNames.EntityNullCheck, () => !property.IsNullable && !property.IsValueType))
                    .Select(property => context.Request.CreateArgumentNullException(property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()))
            )
            .AddCodeStatements(initializationResults.Select(x => x.Value!.ToString()))
            .AddCodeStatements(context.Request.CreateEntityValidationCode())
            .WithChainCall(await context.CreateEntityChainCallAsync().ConfigureAwait(false)));
    }
}
