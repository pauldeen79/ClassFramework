namespace ClassFramework.Pipelines.Reflection.Features;

public class AddPropertiesFeatureBuilder : IReflectionFeatureBuilder
{
    public IPipelineFeature<TypeBaseBuilder, ReflectionContext> Build()
        => new AddPropertiesFeature();
}

public class AddPropertiesFeature : IPipelineFeature<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.AddProperties(GetProperties(context));

        return Result.Continue<TypeBaseBuilder>();
    }

    public IBuilder<IPipelineFeature<TypeBaseBuilder, ReflectionContext>> ToBuilder()
        => new AddPropertiesFeatureBuilder();

    private static IEnumerable<PropertyBuilder> GetProperties(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
        => context.Context.SourceModel.GetPropertiesRecursively().Select
        (
            p => new PropertyBuilder()
                .WithName(p.Name)
                .WithTypeName(p.PropertyType.GetTypeName(p))
                .WithHasGetter(p.GetGetMethod() is not null)
                .WithHasSetter(p.GetSetMethod() is not null)
                .WithHasInitializer(p.IsInitOnly())
                .WithParentTypeFullName(context.Context.MapTypeName(p.DeclaringType.GetParentTypeFullName()))
                .WithIsNullable(p.IsNullable())
                .WithIsValueType(p.PropertyType.IsValueType())
                .WithVisibility(Array.Exists(p.GetAccessors(), m => m.IsPublic).ToVisibility())
                .AddAttributes(p.GetCustomAttributes(true).ToAttributes(
                    x => x.ConvertToDomainAttribute(context.Context.Settings.AttributeInitializeDelegate),
                    context.Context.Settings.CopyAttributes,
                    context.Context.Settings.CopyAttributePredicate))
        );
}
