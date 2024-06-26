﻿namespace ClassFramework.Pipelines.Entity.Components;

public class PartialComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<EntityContext> Build() => new PartialComponent();
}

public class PartialComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> Process(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.WithPartial(context.Request.Settings.CreateAsPartial);

        return Task.FromResult(Result.Continue());
    }
}
