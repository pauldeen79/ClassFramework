namespace ClassFramework.Pipelines.Reflection.Features;

public class AddFieldsComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new AddFieldsComponent();
}

public class AddFieldsComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.AddFields(GetFields(context));

        return Result.Continue<TypeBaseBuilder>();
    }

    private static IEnumerable<FieldBuilder> GetFields(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
        => context.Context.SourceModel.GetFieldsRecursively().Select
        (
            f => new FieldBuilder()
                .WithName(f.Name)
                .WithTypeName(context.Context.GetMappedTypeName(f.FieldType, f))
                .WithStatic(f.IsStatic)
                .WithConstant(f.IsLiteral)
                .WithReadOnly(f.IsInitOnly)
                .WithParentTypeFullName(f.DeclaringType.GetParentTypeFullName())
                .WithIsNullable(f.IsNullable())
                .WithIsValueType(f.FieldType.IsValueType())
                .WithVisibility(f.IsPublic.ToVisibility())
                .AddGenericTypeArguments(f.FieldType.GenericTypeArguments.Select((x, index) => x.ToTypeContainer(f.FieldType, index + 1, context.Context.GetMappedTypeName)))
                .AddAttributes(f.GetCustomAttributes(true).ToAttributes(
                    x => x.ConvertToDomainAttribute(context.Context.InitializeDelegate),
                    context.Context.Settings.CopyAttributes,
                    context.Context.Settings.CopyAttributePredicate))
        );
}
