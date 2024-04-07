﻿namespace ClassFramework.Pipelines.Reflection.Features;

public class AddMethodsComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new AddMethodsComponent();
}

public class AddMethodsComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.AddMethods(GetMethods(context));

        return Result.Continue<TypeBaseBuilder>();
    }

    private static IEnumerable<MethodBuilder> GetMethods(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
        => context.Context.SourceModel.GetMethodsRecursively()
            .Where(m =>
                m.Name != "<Clone>$"
                && !m.Name.StartsWith("get_")
                && !m.Name.StartsWith("set_")
                && m.DeclaringType != typeof(object)
                && m.DeclaringType == context.Context.SourceModel)
            .Select
            (
                m => new MethodBuilder()
                    .WithName(m.Name)
                    .WithReturnTypeName(context.Context.GetMappedTypeName(m.ReturnType, m))
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
                            .WithTypeName(context.Context.GetMappedTypeName(p.ParameterType, m))
                            .WithIsNullable(p.IsNullable())
                            .WithIsValueType(p.ParameterType.IsValueType())
                            .AddAttributes(p.GetCustomAttributes(true).ToAttributes(
                                x => x.ConvertToDomainAttribute(context.Context.InitializeDelegate),
                                context.Context.Settings.CopyAttributes,
                                context.Context.Settings.CopyAttributePredicate))

                    ))
                    .AddAttributes(m.GetCustomAttributes(true).ToAttributes(
                        x => x.ConvertToDomainAttribute(context.Context.InitializeDelegate),
                        context.Context.Settings.CopyAttributes,
                        context.Context.Settings.CopyAttributePredicate))
            );
}