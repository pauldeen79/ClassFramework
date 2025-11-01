namespace ClassFramework.Pipelines.Reflection.Components;

public class AddFieldsComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> ExecuteAsync(ReflectionContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Builder.AddFields(GetFields(context));

            return Result.Success();
        }, token);

    private static IEnumerable<FieldBuilder> GetFields(ReflectionContext context)
        => context.SourceModel.GetFieldsRecursively().Select
        (
            f => new FieldBuilder()
                .WithName(f.Name)
                .WithTypeName(context.GetMappedTypeName(f.FieldType, f))
                .WithStatic(f.IsStatic)
                .WithConstant(f.IsLiteral)
                .WithReadOnly(f.IsInitOnly)
                .WithParentTypeFullName(f.DeclaringType.GetParentTypeFullName())
                .WithVisibility(f.IsPublic.ToVisibility())
                .SetTypeContainerPropertiesFrom(f.IsNullable(), f.FieldType, context.GetMappedTypeName)
                .AddAttributes(f.GetCustomAttributes(true).ToAttributes(
                    x => x.ConvertToDomainAttribute(context.InitializeDelegate),
                    context.Settings.CopyAttributes,
                    context.Settings.CopyAttributePredicate))
        );
}
