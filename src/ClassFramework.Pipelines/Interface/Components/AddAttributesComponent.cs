namespace ClassFramework.Pipelines.Interface.Components;

public class AddAttributesComponent : IPipelineComponent<InterfaceContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ExecuteAsync(InterfaceContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (!context.Settings.CopyAttributes)
            {
                return Result.Continue();
            }

            context.Builder.AddAttributes(context.SourceModel.Attributes
                .Where(x => context.Settings.CopyAttributePredicate?.Invoke(x) ?? true)
                .Select(x => context.MapAttribute(x).ToBuilder()));

            return Result.Success();
        }, token);
}
