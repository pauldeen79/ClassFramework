namespace ClassFramework.Pipelines.Reflection.Features;

public class SetModifiersComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext, TypeBaseBuilder> Build()
        => new SetModifiersComponent();
}

public class SetModifiersComponent : IPipelineComponent<ReflectionContext, TypeBaseBuilder>
{
    public Task<Result> Process(PipelineContext<ReflectionContext, TypeBaseBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Response is IReferenceTypeBuilder referenceTypeBuilder)
        {
            referenceTypeBuilder
                .WithStatic(context.Request.SourceModel.IsAbstract && context.Request.SourceModel.IsSealed)
                .WithSealed(context.Request.SourceModel.IsSealed)
                .WithPartial(context.Request.Settings.CreateAsPartial)
                .WithAbstract(context.Request.SourceModel.IsAbstract);
        }
        
        if (context.Response is IRecordContainerBuilder recordContainerBuilder)
        {
            recordContainerBuilder.WithRecord(context.Request.SourceModel.IsRecord());
        }

        return Task.FromResult(Result.Continue());
    }
}
