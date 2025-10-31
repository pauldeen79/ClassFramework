namespace ClassFramework.Pipelines.Interface.Components;

public class AddPropertiesComponent : IPipelineComponent<InterfaceContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ExecuteAsync(InterfaceContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.AddProperties
            (
                context.GetSourceProperties().Select
                (
                    property => context.CreatePropertyForEntity(property, context.Settings.BuilderAbstractionsTypeConversionMetadataName)
                        .WithHasGetter(property.HasGetter)
                        .WithHasInitializer(false)
                        .WithHasSetter(property.HasSetter && context.Settings.AddSetters)
                )
            );

            return Result.Success();
        }, token);
}
