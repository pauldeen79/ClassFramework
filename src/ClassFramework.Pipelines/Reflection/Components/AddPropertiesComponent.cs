namespace ClassFramework.Pipelines.Reflection.Components;

public class AddPropertiesComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ExecuteAsync(ReflectionContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.AddProperties(GetProperties(context));

            return Result.Success();
        }, token);

    private static IEnumerable<PropertyBuilder> GetProperties(ReflectionContext context)
        => context.SourceModel.GetPropertiesRecursively().Select
        (
            property => new PropertyBuilder()
                .WithName(property.Name)
                .WithTypeName(context.GetMappedTypeName(property.PropertyType, property))
                .WithHasGetter(property.GetGetMethod() is not null)
                .WithHasSetter(property.GetSetMethod() is not null)
                .WithHasInitializer(property.IsInitOnly())
                .WithParentTypeFullName(context.MapTypeName(property.DeclaringType.GetParentTypeFullName()))
                .SetTypeContainerPropertiesFrom(property.IsNullable(), property.PropertyType, context.GetMappedTypeName)
                .WithVisibility(Array.Exists(property.GetAccessors(), methodInfo => methodInfo.IsPublic).ToVisibility())
                .AddAttributes(property.GetCustomAttributes(true).ToAttributes(
                    attribute => attribute.ConvertToDomainAttribute(context.InitializeDelegate),
                    context.Settings.CopyAttributes,
                    context.Settings.CopyAttributePredicate))
        );
}
