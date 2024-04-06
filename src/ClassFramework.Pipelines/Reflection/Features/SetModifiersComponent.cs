namespace ClassFramework.Pipelines.Reflection.Features;

public class SetModifiersComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new SetModifiersComponent();
}

public class SetModifiersComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Model is IReferenceTypeBuilder referenceTypeBuilder)
        {
            referenceTypeBuilder
                .WithStatic(context.Context.SourceModel.IsAbstract && context.Context.SourceModel.IsSealed)
                .WithSealed(context.Context.SourceModel.IsSealed)
                .WithPartial(context.Context.Settings.CreateAsPartial)
                .WithAbstract(context.Context.SourceModel.IsAbstract);
        }
        
        if (context.Model is IRecordContainerBuilder recordContainerBuilder)
        {
            recordContainerBuilder.WithRecord(context.Context.SourceModel.IsRecord());
        }

        return Result.Continue<TypeBaseBuilder>();
    }
}
