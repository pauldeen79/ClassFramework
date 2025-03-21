﻿namespace ClassFramework.Pipelines.Entity.Components;

public class SetRecordComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> ProcessAsync(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.WithRecord(context.Request.Settings.CreateRecord);

        return Task.FromResult(Result.Success());
    }
}
