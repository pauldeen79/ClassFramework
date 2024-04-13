namespace ClassFramework.Pipelines.Reflection.Features;

public class AddPropertiesComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new AddPropertiesComponent();
}

public class AddPropertiesComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Task<Result<TypeBaseBuilder>> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.AddProperties(GetProperties(context));

        return Task.FromResult(Result.Continue<TypeBaseBuilder>());
    }

    private static IEnumerable<PropertyBuilder> GetProperties(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
        => context.Context.SourceModel.GetPropertiesRecursively().Select
        (
            p => new PropertyBuilder()
                .WithName(p.Name)
                .WithTypeName(context.Context.GetMappedTypeName(p.PropertyType, p))
                .WithHasGetter(p.GetGetMethod() is not null)
                .WithHasSetter(p.GetSetMethod() is not null)
                .WithHasInitializer(p.IsInitOnly())
                .WithParentTypeFullName(context.Context.MapTypeName(p.DeclaringType.GetParentTypeFullName()))
                .SetTypeContainerPropertiesFrom(p.IsNullable(), p.PropertyType, context.Context.GetMappedTypeName)
                .WithVisibility(Array.Exists(p.GetAccessors(), m => m.IsPublic).ToVisibility())
                .AddAttributes(p.GetCustomAttributes(true).ToAttributes(
                    x => x.ConvertToDomainAttribute(context.Context.InitializeDelegate),
                    context.Context.Settings.CopyAttributes,
                    context.Context.Settings.CopyAttributePredicate))
        );
}
