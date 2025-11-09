namespace ClassFramework.Pipelines.Reflection.Commands;

public class GenerateTypeFromReflectionCommand : CommandBase<Type>
{
    public GenerateTypeFromReflectionCommand(Type sourceModel, PipelineSettings settings, IFormatProvider formatProvider, CancellationToken cancellationToken)
        : base(sourceModel, settings, formatProvider, cancellationToken)
    {
    }

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    public override bool SourceModelHasNoProperties() => SourceModel.GetProperties().Length == 0;

    public override async Task<Result<TypeBaseBuilder>> ExecuteCommandAsync<TContext>(ICommandService commandService, TContext command, CancellationToken token)
    {
        commandService = ArgumentGuard.IsNotNull(commandService, nameof(commandService));
        command = ArgumentGuard.IsNotNull(command, nameof(command));

        return await commandService.ExecuteAsync<TContext, TypeBaseBuilder>(command, token).ConfigureAwait(false);
    }
}
