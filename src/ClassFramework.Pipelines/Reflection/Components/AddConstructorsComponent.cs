namespace ClassFramework.Pipelines.Reflection.Components;

public class AddConstructorsComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ExecuteAsync(ReflectionContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            if (!context.Settings.CreateConstructors
                || context.Builder is not IConstructorsContainerBuilder constructorsContainerBuilder)
            {
                return Result.Continue();
            }

            constructorsContainerBuilder.AddConstructors(GetConstructors(context));

            return Result.Success();
        }, token);

    private static IEnumerable<ConstructorBuilder> GetConstructors(ReflectionContext context)
        => context.SourceModel.GetConstructors()
            .Select(x => new ConstructorBuilder()
                .AddParameters
                (
                    x.GetParameters().Select
                    (
                        p =>
                        new ParameterBuilder()
                            .WithName(p.Name)
                            .WithTypeName(p.ParameterType.FullName.FixTypeName())
                            .SetTypeContainerPropertiesFrom(p.IsNullable(), p.ParameterType, context.GetMappedTypeName)
                            .AddAttributes(p.GetCustomAttributes(true).ToAttributes(
                                x => x.ConvertToDomainAttribute(context.InitializeDelegate),
                                context.Settings.CopyAttributes,
                                context.Settings.CopyAttributePredicate))
                    )
                )
                .AddAttributes(x.GetCustomAttributes(true).ToAttributes(
                    x => x.ConvertToDomainAttribute(context.InitializeDelegate),
                    context.Settings.CopyAttributes,
                    context.Settings.CopyAttributePredicate))
        );
}
