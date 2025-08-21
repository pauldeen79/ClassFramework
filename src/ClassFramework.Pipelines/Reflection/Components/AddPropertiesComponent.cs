namespace ClassFramework.Pipelines.Reflection.Components;

public class AddPropertiesComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.AddProperties(GetProperties(context));

            return Result.Success();
        }, token);

    private static IEnumerable<PropertyBuilder> GetProperties(PipelineContext<ReflectionContext> context)
        => context.Request.SourceModel.GetPropertiesRecursively().Select
        (
            property => new PropertyBuilder()
                .WithName(property.Name)
                .WithTypeName(context.Request.GetMappedTypeName(property.PropertyType, property))
                .WithHasGetter(property.GetGetMethod() is not null)
                .WithHasSetter(property.GetSetMethod() is not null)
                .WithHasInitializer(property.IsInitOnly())
                .WithParentTypeFullName(context.Request.MapTypeName(property.DeclaringType.GetParentTypeFullName()))
                .SetTypeContainerPropertiesFrom(property.IsNullable(), property.PropertyType, context.Request.GetMappedTypeName)
                .WithVisibility(Array.Exists(property.GetAccessors(), methodInfo => methodInfo.IsPublic).ToVisibility())
                .AddAttributes(property.GetCustomAttributes(true).ToAttributes(
                    attribute => attribute.ConvertToDomainAttribute(context.Request.InitializeDelegate),
                    context.Request.Settings.CopyAttributes,
                    context.Request.Settings.CopyAttributePredicate))
        );
}
