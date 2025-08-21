namespace ClassFramework.Pipelines.Interface.Components;

public class AddPropertiesComponent : IPipelineComponent<InterfaceContext>
{
    public Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            var properties = context.Request.GetSourceProperties().ToArray();

            context.Request.Builder.AddProperties
            (
                properties.Select
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
