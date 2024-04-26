namespace ClassFramework.Pipelines.Reflection.Features;

public class SetModifiersComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new SetModifiersComponent();
}

public class SetModifiersComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Task<Result<TypeBaseBuilder>> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Model is IReferenceTypeBuilder referenceTypeBuilder)
        {
            referenceTypeBuilder
                .WithStatic(context.Request.SourceModel.IsAbstract && context.Request.SourceModel.IsSealed)
                .WithSealed(context.Request.SourceModel.IsSealed)
                .WithPartial(context.Request.Settings.CreateAsPartial)
                .WithAbstract(context.Request.SourceModel.IsAbstract);
        }
        
        if (context.Model is IRecordContainerBuilder recordContainerBuilder)
        {
            recordContainerBuilder.WithRecord(context.Request.SourceModel.IsRecord());
        }

        return Task.FromResult(Result.Continue<TypeBaseBuilder>());
    }
}
