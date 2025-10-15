namespace ClassFramework.Pipelines.Reflection.Components;

public class AddAttributesComponent : IPipelineComponent<ReflectionContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (!context.Request.Settings.CopyAttributes)
            {
                return Result.Continue();
            }

            context.Request.Builder.AddAttributes(context.Request.SourceModel.GetCustomAttributes(true).ToAttributes(
                x => context.Request.MapAttribute(x.ConvertToDomainAttribute(context.Request.InitializeDelegate)),
                context.Request.Settings.CopyAttributes,
                context.Request.Settings.CopyAttributePredicate));

            return Result.Success();
        }, token);
}
