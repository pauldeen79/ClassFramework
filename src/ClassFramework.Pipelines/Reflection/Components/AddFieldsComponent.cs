namespace ClassFramework.Pipelines.Reflection.Components;

public class AddFieldsComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext, TypeBaseBuilder> Build()
        => new AddFieldsComponent();
}

public class AddFieldsComponent : IPipelineComponent<ReflectionContext, TypeBaseBuilder>
{
    public Task<Result> Process(PipelineContext<ReflectionContext, TypeBaseBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        context.Response.AddFields(GetFields(context));

        return Task.FromResult(Result.Continue());
    }

    private static IEnumerable<FieldBuilder> GetFields(PipelineContext<ReflectionContext, TypeBaseBuilder> context)
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
