﻿namespace ClassFramework.Pipelines.Entity.Components;

public class AddPublicParameterlessConstructorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<EntityContext>, IOrderContainer
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public int Order => PipelineStage.Process;

    public async Task<Result> ExecuteAsync(EntityContext context, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Settings.AddPublicParameterlessConstructor)
        {
            return Result.Continue();
        }

        return (await CreateEntityConstructor(context, token)
            .ConfigureAwait(false))
            .OnSuccess(ctorResult => context.Builder.AddConstructors(ctorResult.Value!));
    }

    private async Task<Result<ConstructorBuilder>> CreateEntityConstructor(EntityContext context, CancellationToken token)
    {
        var initializationStatements = new List<Result<string>>();

        foreach (var property in context.GetSourceProperties())
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

    private async Task<Result<string>> GenerateDefaultValueStatement(Property property, EntityContext context, CancellationToken token)
        => (await _evaluator.EvaluateInterpolatedStringAsync
        (
            property.TypeName.FixTypeName().IsCollectionTypeName()
                ? "{property.EntityMemberName} = new {collectionTypeName}<{GenericArguments(property.TypeName)}>();"
                : "{property.EntityMemberName} = {property.DefaultValue};",
            context.FormatProvider,
            new ParentChildContext<EntityContext, Property>(context, property, context.Settings),
            token
        ).ConfigureAwait(false)).Transform(x => x.ToString());
}
