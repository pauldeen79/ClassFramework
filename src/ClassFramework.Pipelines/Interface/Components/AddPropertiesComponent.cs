namespace ClassFramework.Pipelines.Interface.Components;

public class AddPropertiesComponent : IPipelineComponent<InterfaceContext, InterfaceBuilder>
{
    public Task<Result> ExecuteAsync(InterfaceContext context, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            response.AddProperties
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
