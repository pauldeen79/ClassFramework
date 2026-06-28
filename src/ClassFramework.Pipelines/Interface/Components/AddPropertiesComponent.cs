namespace ClassFramework.Pipelines.Interface.Components;

public class AddPropertiesComponent : IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>
{
    public Task<Result> ExecuteAsync(GenerateInterfaceCommand command, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            var properties = command.GetSourceProperties().Select
            (
                property => command.CreatePropertyForEntity(property, command.Settings.BuilderAbstractionsTypeConversionMetadataName)
                    .WithHasGetter(property.HasGetter)
                    .WithHasInitializer(false)
                    .WithHasSetter(property.HasSetter && command.Settings.AddSetters)
            );

            if (!command.Settings.FluentBuilderMethods)
            {
                response.AddProperties(properties);
            }
            else
            {
                response.AddMethods(properties.SelectMany(property => CommandBase.ConvertPropertyToMethods(
                    property,
                    property.TypeName,
                    property.IsNullable,
                    property.IsValueType)));
            }

            return Result.Success();
        }, token);
}
