namespace ClassFramework.Pipelines.Reflection.Components;

public class AddAttributesComponent : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand context, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            if (!context.Settings.CopyAttributes)
            {
                return Result.Continue();
            }

            response.AddAttributes(context.SourceModel.GetCustomAttributes(true).ToAttributes(
                x => context.MapAttribute(x.ConvertToDomainAttribute(context.InitializeDelegate)),
                context.Settings.CopyAttributes,
                context.Settings.CopyAttributePredicate));

            return Result.Success();
        }, token);
}
