namespace ClassFramework.Pipelines.Entity.Components;

public class AddPublicParameterlessConstructorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AddPublicParameterlessConstructor)
        {
            return Result.Success();
        }

        var ctorResult = await CreateEntityConstructor(context, token).ConfigureAwait(false);
        if (!ctorResult.IsSuccessful())
        {
            return ctorResult;
        }

        context.Request.Builder.AddConstructors(ctorResult.Value!);

        return Result.Success();
    }

    private async Task<Result<ConstructorBuilder>> CreateEntityConstructor(PipelineContext<EntityContext> context, CancellationToken token)
    {
        var initializationStatements = new List<Result<string>>();

        foreach (var property in context.Request.GetSourceProperties())
        {
            var result = await GenerateDefaultValueStatement(property, context, token).ConfigureAwait(false);
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

    private async Task<Result<string>> GenerateDefaultValueStatement(Property property, PipelineContext<EntityContext> context, CancellationToken token)
        => (await _evaluator.EvaluateInterpolatedStringAsync
        (
            property.TypeName.FixTypeName().IsCollectionTypeName()
                ? "{property.EntityMemberName} = new {collectionTypeName}<{GenericArguments(property.TypeName)}>();"
                : "{property.EntityMemberName} = {property.DefaultValue};",
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<EntityContext>, Property>(context, property, context.Request.Settings),
            token
        ).ConfigureAwait(false)).Transform(x => x.ToString());
}
