namespace ClassFramework.Pipelines;

public class PipelineFeatureWrapper<TModel, TContext> : IPipelineFeature<TModel, TContext>
{
    private readonly Func<IPipelineFeature<TModel>> _featureCreateDelegate;

    public PipelineFeatureWrapper(Func<IPipelineFeature<TModel>> featureCreateDelegate)
        => _featureCreateDelegate = featureCreateDelegate.IsNotNull(nameof(featureCreateDelegate));

    public Result<TModel> Process(PipelineContext<TModel, TContext> context)
    {
        context = context.IsNotNull(nameof(context));

        return _featureCreateDelegate().Process(new PipelineContext<TModel>(context.Model));
    }

    public IBuilder<IPipelineFeature<TModel, TContext>> ToBuilder()
        => new PipelineFeatureWrapperBuilder<TModel, TContext>(_featureCreateDelegate);
}

public class PipelineFeatureWrapperBuilder<TModel, TContext> : IBuilder<IPipelineFeature<TModel, TContext>>
{
    private readonly Func<IPipelineFeature<TModel>> _featureCreateDelegate;

    public PipelineFeatureWrapperBuilder(Func<IPipelineFeature<TModel>> featureCreateDelegate)
        => _featureCreateDelegate = featureCreateDelegate.IsNotNull(nameof(featureCreateDelegate));

    public IPipelineFeature<TModel, TContext> Build()
        => new PipelineFeatureWrapper<TModel, TContext>(_featureCreateDelegate);
}
