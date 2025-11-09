namespace ClassFramework.Pipelines.Reflection.Components;

public class AddMethodsComponent : IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>
{
    public Task<Result> ExecuteAsync(GenerateTypeFromReflectionCommand context, TypeBaseBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));
            response = response.IsNotNull(nameof(response));

            response.AddMethods(GetMethods(context));

            return Result.Success();
        }, token);

    private static IEnumerable<MethodBuilder> GetMethods(GenerateTypeFromReflectionCommand context)
        => context.SourceModel.GetMethodsRecursively()
            .Where(methodInfo =>
                methodInfo.Name != "<Clone>$"
                && !methodInfo.Name.StartsWith("get_")
                && !methodInfo.Name.StartsWith("set_")
                && methodInfo.DeclaringType != typeof(object)
                && methodInfo.DeclaringType == context.SourceModel)
            .Select
            (
                methodInfo => new MethodBuilder()
                    .WithName(methodInfo.Name)
                    .WithReturnTypeName(context.GetMappedTypeName(methodInfo.ReturnType, methodInfo))
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
                            .WithTypeName(context.GetMappedTypeName(parameter.ParameterType, methodInfo))
                            .SetTypeContainerPropertiesFrom(parameter.IsNullable(), parameter.ParameterType, context.GetMappedTypeName)
                            .AddAttributes(parameter.GetCustomAttributes(true).ToAttributes(
                                attribute => attribute.ConvertToDomainAttribute(context.InitializeDelegate),
                                context.Settings.CopyAttributes,
                                context.Settings.CopyAttributePredicate))

                    ))
                    .AddAttributes(methodInfo.GetCustomAttributes(true).ToAttributes(
                        attribute => attribute.ConvertToDomainAttribute(context.InitializeDelegate),
                        context.Settings.CopyAttributes,
                        context.Settings.CopyAttributePredicate))
            );
}
