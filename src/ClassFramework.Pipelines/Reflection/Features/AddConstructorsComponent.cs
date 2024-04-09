namespace ClassFramework.Pipelines.Reflection.Features;

public class AddConstructorsComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new AddConstructorsComponent();
}

public class AddConstructorsComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CreateConstructors
            || context.Model is not IConstructorsContainerBuilder constructorsContainerBuilder)
        {
            return Result.Continue<TypeBaseBuilder>();
        }

        constructorsContainerBuilder.AddConstructors(GetConstructors(context));

        return Result.Continue<TypeBaseBuilder>();
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
                            .WithIsNullable(p.IsNullable())
                            .WithIsValueType(p.ParameterType.IsValueType())
                            .AddGenericTypeArguments(p.ParameterType.GenericTypeArguments.Select((x, index) => x.ToTypeContainer(p.ParameterType, index + 1, context.Context.GetMappedTypeName)))
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
