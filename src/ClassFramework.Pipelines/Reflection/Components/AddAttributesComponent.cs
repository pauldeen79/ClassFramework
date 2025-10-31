namespace ClassFramework.Pipelines.Reflection.Components;

public class AddAttributesComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ExecuteAsync(ReflectionContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (!context.Settings.CopyAttributes)
            {
                return Result.Continue();
            }

            context.Builder.AddAttributes(context.SourceModel.GetCustomAttributes(true).ToAttributes(
                x => context.MapAttribute(x.ConvertToDomainAttribute(context.InitializeDelegate)),
                context.Settings.CopyAttributes,
                context.Settings.CopyAttributePredicate));

            return Result.Success();
        }, token);
}
