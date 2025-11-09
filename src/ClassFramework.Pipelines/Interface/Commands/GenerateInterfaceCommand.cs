namespace ClassFramework.Pipelines.Interface.Commands;

public class GenerateInterfaceCommand(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider) : CommandBase<TypeBase>(sourceModel, settings, formatProvider)
{
    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    public IEnumerable<Property> GetSourceProperties()
        => SourceModel.Properties.Where(x => SourceModel.IsMemberValidForBuilderClass(x, Settings));

    public override bool SourceModelHasNoProperties() => SourceModel.Properties.Count == 0;

    public override async Task<Result<TypeBaseBuilder>> ExecuteCommandAsync<TContext>(ICommandService commandService, TContext command, CancellationToken token)
    {
        commandService = ArgumentGuard.IsNotNull(commandService, nameof(commandService));
        command = ArgumentGuard.IsNotNull(command, nameof(command));

        return (await commandService.ExecuteAsync<TContext, InterfaceBuilder>(command, token).ConfigureAwait(false))
            .TryCast<TypeBaseBuilder>();
    }
}
