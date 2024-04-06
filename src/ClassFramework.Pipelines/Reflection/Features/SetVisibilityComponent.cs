namespace ClassFramework.Pipelines.Reflection.Features;

public class SetVisibilityComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new SetVisibilityComponent();
}

public class SetVisibilityComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Context.SourceModel.IsPublic)
        {
            context.Model.WithVisibility(Visibility.Public);
        }
        else
        {
            context.Model.WithVisibility(context.Context.SourceModel.IsNotPublic
                ? Visibility.Internal
                : Visibility.Private);
        }

        return Result.Continue<TypeBaseBuilder>();
    }
}
