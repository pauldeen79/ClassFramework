namespace ClassFramework.Pipelines.Reflection.Components;

public class AddMethodsComponent : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand command, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            response.AddMethods(GetMethods(command));

            return Result.Success();
        }, token);

    private static IEnumerable<MethodBuilder> GetMethods(GenerateTypeFromReflectionCommand command)
        => command.SourceModel.GetMethodsRecursively()
            .Where(methodInfo =>
                methodInfo.Name != "<Clone>$"
                && !methodInfo.Name.StartsWith("get_")
                && !methodInfo.Name.StartsWith("set_")
                && methodInfo.DeclaringType != typeof(object)
                && methodInfo.DeclaringType == command.SourceModel)
            .Select
            (
                methodInfo => new MethodBuilder()
                    .WithName(methodInfo.Name)
                    .WithReturnTypeName(command.GetMappedTypeName(methodInfo.ReturnType, methodInfo))
                    .WithVisibility(methodInfo.IsPublic.ToVisibility())
                    .WithStatic(methodInfo.IsStatic)
                    .WithVirtual(methodInfo.IsVirtual)
                    .WithAbstract(methodInfo.IsAbstract)
                    .WithParentTypeFullName(methodInfo.DeclaringType.GetParentTypeFullName())
                    .WithReturnTypeIsNullable(methodInfo.ReturnTypeIsNullable())
                    .WithReturnTypeIsValueType(methodInfo.ReturnType.IsValueType())
                    .AddParameters(methodInfo.GetParameters().Select
                    (
                        parameter => new ParameterBuilder()
                            .WithName(parameter.Name)
                            .WithTypeName(command.GetMappedTypeName(parameter.ParameterType, methodInfo))
                            .SetTypeContainerPropertiesFrom(parameter.IsNullable(), parameter.ParameterType, command.GetMappedTypeName)
                            .AddAttributes(parameter.GetCustomAttributes(true).ToAttributes(
                                attribute => attribute.ConvertToDomainAttribute(command.InitializeDelegate),
                                command.Settings.CopyAttributes,
                                command.Settings.CopyAttributePredicate))

                    ))
                    .AddAttributes(methodInfo.GetCustomAttributes(true).ToAttributes(
                        attribute => attribute.ConvertToDomainAttribute(command.InitializeDelegate),
                        command.Settings.CopyAttributes,
                        command.Settings.CopyAttributePredicate))
            );
}
