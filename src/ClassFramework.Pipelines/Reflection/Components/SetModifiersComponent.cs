namespace ClassFramework.Pipelines.Reflection.Components;

public class SetModifiersComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext> Build()
        => new SetModifiersComponent();
}

public class SetModifiersComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.Builder is IReferenceTypeBuilder referenceTypeBuilder)
        {
            referenceTypeBuilder
                .WithStatic(context.Request.SourceModel.IsAbstract && context.Request.SourceModel.IsSealed)
                .WithSealed(context.Request.SourceModel.IsSealed)
                .WithPartial(context.Request.Settings.CreateAsPartial)
                .WithAbstract(context.Request.SourceModel.IsAbstract);
        }

        if (context.Request.Builder is IRecordContainerBuilder recordContainerBuilder)
        {
            recordContainerBuilder.WithRecord(context.Request.SourceModel.IsRecord());
        }

        return Task.FromResult(Result.Success());
    }
}
