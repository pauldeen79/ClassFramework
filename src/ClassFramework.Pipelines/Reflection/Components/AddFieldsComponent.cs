namespace ClassFramework.Pipelines.Reflection.Components;

public class AddFieldsComponent : IPipelineComponent<ReflectionContext>, IOrderContainer
{
    public int Order => PipelineStage.Process;

    public Task<Result> ProcessAsync(PipelineContext<ReflectionContext> context, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            context.Request.Builder.AddFields(GetFields(context));

            return Result.Success();
        }, token);

    private static IEnumerable<FieldBuilder> GetFields(PipelineContext<ReflectionContext> context)
        => context.Request.SourceModel.GetFieldsRecursively().Select
        (
            f => new FieldBuilder()
                .WithName(f.Name)
                .WithTypeName(context.Request.GetMappedTypeName(f.FieldType, f))
                .WithStatic(f.IsStatic)
                .WithConstant(f.IsLiteral)
                .WithReadOnly(f.IsInitOnly)
                .WithParentTypeFullName(f.DeclaringType.GetParentTypeFullName())
                .WithVisibility(f.IsPublic.ToVisibility())
                .SetTypeContainerPropertiesFrom(f.IsNullable(), f.FieldType, context.Request.GetMappedTypeName)
                .AddAttributes(f.GetCustomAttributes(true).ToAttributes(
                    x => x.ConvertToDomainAttribute(context.Request.InitializeDelegate),
                    context.Request.Settings.CopyAttributes,
                    context.Request.Settings.CopyAttributePredicate))
        );
}
