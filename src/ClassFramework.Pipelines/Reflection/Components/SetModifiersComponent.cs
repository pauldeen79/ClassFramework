namespace ClassFramework.Pipelines.Reflection.Components;

public class SetModifiersComponent : IPipelineComponent<ReflectionContext, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(ReflectionContext context, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            if (response is IReferenceTypeBuilder referenceTypeBuilder)
            {
                referenceTypeBuilder
                    .WithStatic(context.SourceModel.IsAbstract && context.SourceModel.IsSealed)
                    .WithSealed(context.SourceModel.IsSealed)
                    .WithPartial(context.Settings.CreateAsPartial)
                    .WithAbstract(context.SourceModel.IsAbstract);
            }

            if (response is IRecordContainerBuilder recordContainerBuilder)
            {
                recordContainerBuilder.WithRecord(context.SourceModel.IsRecord());
            }

            return Result.Success();
        }, token);
}
