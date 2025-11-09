namespace ClassFramework.Pipelines.Entity.Components;

public class AddPublicParameterlessConstructorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateEntityCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        if (!context.Settings.AddPublicParameterlessConstructor)
        {
            return Result.Continue();
        }

        return (await CreateEntityConstructorAsync(context, token)
            .ConfigureAwait(false))
            .OnSuccess(ctorResult => response.AddConstructors(ctorResult.Value!));
    }

    private async Task<Result<ConstructorBuilder>> CreateEntityConstructorAsync(GenerateEntityCommand context, CancellationToken token)
    {
        var initializationStatements = new List<Result<string>>();

        foreach (var property in context.GetSourceProperties())
        {
            var result = await GenerateDefaultValueStatementAsync(property, context, token).ConfigureAwait(false);
            initializationStatements.Add(result);
            if (!result.IsSuccessful())
            {
                break;
            }
        }

        var errorResult = initializationStatements.Find(x => !x.IsSuccessful());
        if (errorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(errorResult);
        }

        return Result.Success(new ConstructorBuilder()
            .AddCodeStatements(initializationStatements.Select(x => x.Value!)));
    }

    private async Task<Result<string>> GenerateDefaultValueStatementAsync(Property property, GenerateEntityCommand context, CancellationToken token)
        => (await _evaluator.EvaluateInterpolatedStringAsync
        (
            property.TypeName.FixTypeName().IsCollectionTypeName()
                ? "{property.EntityMemberName} = new {collectionTypeName}<{GenericArguments(property.TypeName)}>();"
                : "{property.EntityMemberName} = {property.DefaultValue};",
            context.FormatProvider,
            new ParentChildContext<GenerateEntityCommand, Property>(context, property, context.Settings),
            token
        ).ConfigureAwait(false)).Transform(x => x.ToString());
}
