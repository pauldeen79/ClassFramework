namespace ClassFramework.Pipelines.Reflection.Components;

public class AddConstructorsComponent : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand command, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            if (!command.Settings.CreateConstructors
                || response is not IConstructorsContainerBuilder constructorsContainerBuilder)
            {
                return Result.Continue();
            }

            constructorsContainerBuilder.AddConstructors(GetConstructors(command));

            return Result.Success();
        }, token);

    private static IEnumerable<ConstructorBuilder> GetConstructors(GenerateTypeFromReflectionCommand command)
        => command.SourceModel.GetConstructors()
            .Select(x => new ConstructorBuilder()
                .AddParameters
                (
                    x.GetParameters().Select
                    (
                        p =>
                        new ParameterBuilder()
                            .WithName(p.Name)
                            .WithTypeName(p.ParameterType.FullName.FixTypeName())
                            .SetTypeContainerPropertiesFrom(p.IsNullable(), p.ParameterType, command.GetMappedTypeName)
                            .AddAttributes(p.GetCustomAttributes(true).ToAttributes(
                                x => x.ConvertToDomainAttribute(command.InitializeDelegate),
                                command.Settings.CopyAttributes,
                                command.Settings.CopyAttributePredicate))
                    )
                )
                .AddAttributes(x.GetCustomAttributes(true).ToAttributes(
                    x => x.ConvertToDomainAttribute(command.InitializeDelegate),
                    command.Settings.CopyAttributes,
                    command.Settings.CopyAttributePredicate))
        );
}
