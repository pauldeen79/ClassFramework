namespace ClassFramework.Pipelines.Interface.Components;

public class AddPropertiesComponent : IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>
{
    public Task<Result> ExecuteAsync(GenerateInterfaceCommand context, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

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
