
namespace ClassFramework.Pipelines.Reflection.Features;

public class AddAttributesFeatureBuilder : IReflectionFeatureBuilder
{
    public IPipelineFeature<TypeBaseBuilder, ReflectionContext> Build()
        => new AddAttributesFeature();
}

public class AddAttributesFeature : IPipelineFeature<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CopyAttributes)
        {
            return Result.Continue<TypeBaseBuilder>();
        }

        context.Model.AddAttributes(context.Context.SourceModel.GetCustomAttributes(true).ToAttributes(
            x => context.Context.MapAttribute(x.ConvertToDomainAttribute(context.Context.GetInitializeDelegate())),
            context.Context.Settings.CopyAttributes,
            context.Context.Settings.CopyAttributePredicate));

        return Result.Continue<TypeBaseBuilder>();
    }

    public IBuilder<IPipelineFeature<TypeBaseBuilder, ReflectionContext>> ToBuilder()
        => new AddAttributesFeatureBuilder();
}
