namespace ClassFramework.Pipelines.Entity.Components;

public class AddPublicParameterlessConstructorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AddPublicParameterlessConstructor)
        {
            return Task.FromResult(Result.Success());
        }

        var ctorResult = CreateEntityConstructor(context);
        if (!ctorResult.IsSuccessful())
        {
            return Task.FromResult<Result>(ctorResult);
        }

        context.Request.Builder.AddConstructors(ctorResult.Value!);

        return Task.FromResult(Result.Success());
    }

    private Result<ConstructorBuilder> CreateEntityConstructor(PipelineContext<EntityContext> context)
    {
        var initializationStatements = context.Request.SourceModel.Properties
            .Where(x => context.Request.SourceModel.IsMemberValidForBuilderClass(x, context.Request.Settings))
            .Select(x => GenerateDefaultValueStatement(x, context))
            .ToArray();

        var errorResult = Array.Find(initializationStatements, x => !x.IsSuccessful());
        if (errorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(errorResult);
        }

        return Result.Success(new ConstructorBuilder()
            .AddStringCodeStatements(initializationStatements.Select(x => x.Value!))
            );
    }

    private Result<string> GenerateDefaultValueStatement(Property property, PipelineContext<EntityContext> context)
        => _evaluator.Parse
        (
            property.TypeName.FixTypeName().IsCollectionTypeName()
                ? "{$property.EntityMemberName} = new {$collectionTypeName}<{GenericArguments($property.TypeName)}>();"
                : "{$property.EntityMemberName} = {$property.DefaultValue};",
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<EntityContext>, Property>(context, property, context.Request.Settings)
        ).Transform(x => x.ToString());
}
