namespace ClassFramework.Pipelines.Reflection.Components;

public class AddFieldsComponent : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand command, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.AddFields(GetFields(command));

            return Result.Success();
        }, token);

    private static IEnumerable<FieldBuilder> GetFields(GenerateTypeFromReflectionCommand command)
        => command.SourceModel.GetFieldsRecursively().Select
        (
            f => new FieldBuilder()
                .WithName(f.Name)
                .WithTypeName(command.GetMappedTypeName(f.FieldType, f))
                .WithStatic(f.IsStatic)
                .WithConstant(f.IsLiteral)
                .WithReadOnly(f.IsInitOnly)
                .WithParentTypeFullName(f.DeclaringType.GetParentTypeFullName())
                .WithVisibility(f.IsPublic.ToVisibility())
                .SetTypeContainerPropertiesFrom(f.IsNullable(), f.FieldType, command.GetMappedTypeName)
                .AddAttributes(f.GetCustomAttributes(true).ToAttributes(
                    x => x.ConvertToDomainAttribute(command.InitializeDelegate),
                    command.Settings.CopyAttributes,
                    command.Settings.CopyAttributePredicate))
        );
}
