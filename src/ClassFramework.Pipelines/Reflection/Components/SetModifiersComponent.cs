namespace ClassFramework.Pipelines.Reflection.Components;

public class SetModifiersComponent : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand command, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            if (response is IReferenceTypeBuilder referenceTypeBuilder)
            {
                referenceTypeBuilder
                    .WithStatic(command.SourceModel.IsAbstract && command.SourceModel.IsSealed)
                    .WithSealed(command.SourceModel.IsSealed)
                    .WithPartial(command.Settings.CreateAsPartial)
                    .WithAbstract(command.SourceModel.IsAbstract);
            }

            if (response is IRecordContainerBuilder recordContainerBuilder)
            {
                recordContainerBuilder.WithRecord(command.SourceModel.IsRecord());
            }

            return Result.Success();
        }, token);
}
