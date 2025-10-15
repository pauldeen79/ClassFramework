namespace ClassFramework.Pipelines.Interface.Components;

public class AddPropertiesComponent : IPipelineComponent<InterfaceContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.AddProperties
            (
                context.Request.GetSourceProperties().Select
                (
                    property => context.Request.CreatePropertyForEntity(property, context.Request.Settings.BuilderAbstractionsTypeConversionMetadataName)
                        .WithHasGetter(property.HasGetter)
                        .WithHasInitializer(false)
                        .WithHasSetter(property.HasSetter && context.Request.Settings.AddSetters)
                )
            );

            return Result.Success();
        }, token);
}
