﻿namespace ClassFramework.Pipelines.Reflection.Components;

public class AddMethodsComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.AddMethods(GetMethods(context));

        return Task.FromResult(Result.Success());
    }

    private static IEnumerable<MethodBuilder> GetMethods(PipelineContext<ReflectionContext> context)
        => context.Request.SourceModel.GetMethodsRecursively()
            .Where(m =>
                m.Name != "<Clone>$"
                && !m.Name.StartsWith("get_")
                && !m.Name.StartsWith("set_")
                && m.DeclaringType != typeof(object)
                && m.DeclaringType == context.Request.SourceModel)
            .Select
            (
                m => new MethodBuilder()
                    .WithName(m.Name)
                    .WithReturnTypeName(context.Request.GetMappedTypeName(m.ReturnType, m))
                    .WithVisibility(m.IsPublic.ToVisibility())
                    .WithStatic(m.IsStatic)
                    .WithVirtual(m.IsVirtual)
                    .WithAbstract(m.IsAbstract)
                    .WithParentTypeFullName(m.DeclaringType.GetParentTypeFullName())
                    .WithReturnTypeIsNullable(m.ReturnTypeIsNullable())
                    .WithReturnTypeIsValueType(m.ReturnType.IsValueType())
                    .AddParameters(m.GetParameters().Select
                    (
                        p => new ParameterBuilder()
                            .WithName(p.Name)
                            .WithTypeName(context.Request.GetMappedTypeName(p.ParameterType, m))
                            .SetTypeContainerPropertiesFrom(p.IsNullable(), p.ParameterType, context.Request.GetMappedTypeName)
                            .AddAttributes(p.GetCustomAttributes(true).ToAttributes(
                                x => x.ConvertToDomainAttribute(context.Request.InitializeDelegate),
                                context.Request.Settings.CopyAttributes,
                                context.Request.Settings.CopyAttributePredicate))

                    ))
                    .AddAttributes(m.GetCustomAttributes(true).ToAttributes(
                        x => x.ConvertToDomainAttribute(context.Request.InitializeDelegate),
                        context.Request.Settings.CopyAttributes,
                        context.Request.Settings.CopyAttributePredicate))
            );
}
