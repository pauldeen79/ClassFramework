namespace ClassFramework.Pipelines.Reflection.Components;

public class AddConstructorsComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext, TypeBaseBuilder> Build()
        => new AddConstructorsComponent();
}

public class AddConstructorsComponent : IPipelineComponent<ReflectionContext, TypeBaseBuilder>
{
    public Task<Result> Process(PipelineContext<ReflectionContext, TypeBaseBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CreateConstructors
            || context.Response is not IConstructorsContainerBuilder constructorsContainerBuilder)
        {
            return Task.FromResult(Result.Continue());
        }

        constructorsContainerBuilder.AddConstructors(GetConstructors(context));

        return Task.FromResult(Result.Continue());
    }

    private static IEnumerable<ConstructorBuilder> GetConstructors(PipelineContext<ReflectionContext, TypeBaseBuilder> context)
        => context.Request.SourceModel.GetConstructors()
            .Select(x => new ConstructorBuilder()
                .AddParameters
                (
                    x.GetParameters().Select
                    (
                        p =>
                        new ParameterBuilder()
                            .WithName(p.Name)
                            .WithTypeName(p.ParameterType.FullName.FixTypeName())
                            .SetTypeContainerPropertiesFrom(p.IsNullable(), p.ParameterType, context.Request.GetMappedTypeName)
                            .AddAttributes(p.GetCustomAttributes(true).ToAttributes(
                                x => x.ConvertToDomainAttribute(context.Request.InitializeDelegate),
                                context.Request.Settings.CopyAttributes,
                                context.Request.Settings.CopyAttributePredicate))
                    )
                )
                .AddAttributes(x.GetCustomAttributes(true).ToAttributes(
                    x => x.ConvertToDomainAttribute(context.Request.InitializeDelegate),
                    context.Request.Settings.CopyAttributes,
                    context.Request.Settings.CopyAttributePredicate))
        );
}
