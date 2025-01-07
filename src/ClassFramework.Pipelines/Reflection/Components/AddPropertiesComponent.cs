namespace ClassFramework.Pipelines.Reflection.Components;

public class AddPropertiesComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext> Build()
        => new AddPropertiesComponent();
}

public class AddPropertiesComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> Process(PipelineContext<ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Request.Builder.AddProperties(GetProperties(context));

        return Task.FromResult(Result.Success());
    }

    private static IEnumerable<PropertyBuilder> GetProperties(PipelineContext<ReflectionContext> context)
        => context.Request.SourceModel.GetPropertiesRecursively().Select
        (
            p => new PropertyBuilder()
                .WithName(p.Name)
                .WithTypeName(context.Request.GetMappedTypeName(p.PropertyType, p))
                .WithHasGetter(p.GetGetMethod() is not null)
                .WithHasSetter(p.GetSetMethod() is not null)
                .WithHasInitializer(p.IsInitOnly())
                .WithParentTypeFullName(context.Request.MapTypeName(p.DeclaringType.GetParentTypeFullName()))
                .SetTypeContainerPropertiesFrom(p.IsNullable(), p.PropertyType, context.Request.GetMappedTypeName)
                .WithVisibility(Array.Exists(p.GetAccessors(), m => m.IsPublic).ToVisibility())
                .AddAttributes(p.GetCustomAttributes(true).ToAttributes(
                    x => x.ConvertToDomainAttribute(context.Request.InitializeDelegate),
                    context.Request.Settings.CopyAttributes,
                    context.Request.Settings.CopyAttributePredicate))
        );
}
