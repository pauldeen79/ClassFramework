namespace ClassFramework.Pipelines.Reflection.Components;

public class SetModifiersComponent : IPipelineComponent<ReflectionContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ExecuteAsync(ReflectionContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (context.Builder is IReferenceTypeBuilder referenceTypeBuilder)
            {
                referenceTypeBuilder
                    .WithStatic(context.SourceModel.IsAbstract && context.SourceModel.IsSealed)
                    .WithSealed(context.SourceModel.IsSealed)
                    .WithPartial(context.Settings.CreateAsPartial)
                    .WithAbstract(context.SourceModel.IsAbstract);
            }

            if (context.Builder is IRecordContainerBuilder recordContainerBuilder)
            {
                recordContainerBuilder.WithRecord(context.SourceModel.IsRecord());
            }

            return Result.Success();
        }, token);
}
