namespace ClassFramework.Pipelines.Reflection.Components;

public class AddMethodsComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.AddMethods(GetMethods(context));

            return Result.Success();
        }, token);

    private static IEnumerable<MethodBuilder> GetMethods(PipelineContext<ReflectionContext> context)
        => context.Request.SourceModel.GetMethodsRecursively()
            .Where(methodInfo =>
                methodInfo.Name != "<Clone>$"
                && !methodInfo.Name.StartsWith("get_")
                && !methodInfo.Name.StartsWith("set_")
                && methodInfo.DeclaringType != typeof(object)
                && methodInfo.DeclaringType == context.Request.SourceModel)
            .Select
            (
                methodInfo => new MethodBuilder()
                    .WithName(methodInfo.Name)
                    .WithReturnTypeName(context.Request.GetMappedTypeName(methodInfo.ReturnType, methodInfo))
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
                            .WithTypeName(context.Request.GetMappedTypeName(parameter.ParameterType, methodInfo))
                            .SetTypeContainerPropertiesFrom(parameter.IsNullable(), parameter.ParameterType, context.Request.GetMappedTypeName)
                            .AddAttributes(parameter.GetCustomAttributes(true).ToAttributes(
                                attribute => attribute.ConvertToDomainAttribute(context.Request.InitializeDelegate),
                                context.Request.Settings.CopyAttributes,
                                context.Request.Settings.CopyAttributePredicate))

                    ))
                    .AddAttributes(methodInfo.GetCustomAttributes(true).ToAttributes(
                        attribute => attribute.ConvertToDomainAttribute(context.Request.InitializeDelegate),
                        context.Request.Settings.CopyAttributes,
                        context.Request.Settings.CopyAttributePredicate))
            );
}
