﻿namespace ClassFramework.Pipelines.Reflection.Features;

public class AddConstructorsComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new AddConstructorsComponent();
}

public class AddConstructorsComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Task<Result<TypeBaseBuilder>> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CreateConstructors
            || context.Model is not IConstructorsContainerBuilder constructorsContainerBuilder)
        {
            return Task.FromResult(Result.Continue<TypeBaseBuilder>());
        }

        constructorsContainerBuilder.AddConstructors(GetConstructors(context));

        return Task.FromResult(Result.Continue<TypeBaseBuilder>());
    }

    private static IEnumerable<ConstructorBuilder> GetConstructors(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
        => context.Context.SourceModel.GetConstructors()
            .Select(x => new ConstructorBuilder()
                .AddParameters
                (
                    x.GetParameters().Select
                    (
                        p =>
                        new ParameterBuilder()
                            .WithName(p.Name)
                            .WithTypeName(p.ParameterType.FullName.FixTypeName())
                            .SetTypeContainerPropertiesFrom(p.IsNullable(), p.ParameterType, context.Context.GetMappedTypeName)
                            .AddAttributes(p.GetCustomAttributes(true).ToAttributes(
                                x => x.ConvertToDomainAttribute(context.Context.InitializeDelegate),
                                context.Context.Settings.CopyAttributes,
                                context.Context.Settings.CopyAttributePredicate))
                    )
                )
                .AddAttributes(x.GetCustomAttributes(true).ToAttributes(
                    x => x.ConvertToDomainAttribute(context.Context.InitializeDelegate),
                    context.Context.Settings.CopyAttributes,
                    context.Context.Settings.CopyAttributePredicate))
        );
}
