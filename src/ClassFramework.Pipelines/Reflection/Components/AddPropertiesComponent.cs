namespace ClassFramework.Pipelines.Reflection.Components;

public class AddPropertiesComponent : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand command, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.AddProperties(GetProperties(command));

            return Result.Success();
        }, token);

    private static IEnumerable<PropertyBuilder> GetProperties(GenerateTypeFromReflectionCommand command)
        => command.SourceModel.GetPropertiesRecursively().Select
        (
            property => new PropertyBuilder()
                .WithName(property.Name)
                .WithTypeName(command.GetMappedTypeName(property.PropertyType, property))
                .WithHasGetter(property.GetGetMethod() is not null)
                .WithHasSetter(property.GetSetMethod() is not null)
                .WithHasInitializer(property.IsInitOnly())
                .WithParentTypeFullName(command.MapTypeName(property.DeclaringType.GetParentTypeFullName()))
                .SetTypeContainerPropertiesFrom(property.IsNullable(), property.PropertyType, command.GetMappedTypeName)
                .WithVisibility(Array.Exists(property.GetAccessors(), methodInfo => methodInfo.IsPublic).ToVisibility())
                .AddAttributes(property.GetCustomAttributes(true).ToAttributes(
                    attribute => attribute.ConvertToDomainAttribute(command.InitializeDelegate),
                    command.Settings.CopyAttributes,
                    command.Settings.CopyAttributePredicate))
        );
}
