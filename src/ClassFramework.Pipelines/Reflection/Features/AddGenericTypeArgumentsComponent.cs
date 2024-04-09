namespace ClassFramework.Pipelines.Reflection.Features;

public class AddGenericTypeArgumentsComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new AddGenericTypeArgumentsComponent();
}

public class AddGenericTypeArgumentsComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Result<TypeBaseBuilder> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context)
    {
        context = context.IsNotNull(nameof(context));

        context.Model.AddGenericTypeArguments(context.Context.SourceModel.GetGenericTypeArgumentTypeNames());

        return Result.Continue<TypeBaseBuilder>();
    }
}
