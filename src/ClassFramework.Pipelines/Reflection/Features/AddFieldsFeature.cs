namespace ClassFramework.Pipelines.Reflection.Features;

public class AddFieldsFeatureBuilder : IReflectionFeatureBuilder
{
    public IPipelineFeature<TypeBaseBuilder, ReflectionContext> Build()
        => new AddFieldsFeature();
}

public class AddFieldsFeature : IPipelineFeature<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.AddFields(GetFields(context));

        return Result.Continue<TypeBaseBuilder>();
    }

    public IBuilder<IPipelineFeature<TypeBaseBuilder, ReflectionContext>> ToBuilder()
        => new AddFieldsFeatureBuilder();

    private static IEnumerable<FieldBuilder> GetFields(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
        => context.Context.SourceModel.GetFieldsRecursively().Select
        (
            f => new FieldBuilder()
                .WithName(f.Name)
                .WithTypeName(f.FieldType.GetTypeName(f))
                .WithStatic(f.IsStatic)
                .WithConstant(f.IsLiteral)
                .WithReadOnly(f.IsInitOnly)
                .WithParentTypeFullName(f.DeclaringType.GetParentTypeFullName())
                .WithIsNullable(f.IsNullable())
                .WithIsValueType(f.FieldType.IsValueType())
                .WithVisibility(f.IsPublic.ToVisibility())
                .AddAttributes(f.GetCustomAttributes(true).ToAttributes(
                    x => x.ConvertToDomainAttribute(context.Context.InitializeDelegate),
                    context.Context.Settings.CopyAttributes,
                    context.Context.Settings.CopyAttributePredicate))
        );
}
