namespace ClassFramework.Pipelines.Reflection.Components;

public class AddAttributesComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext> Build()
        => new AddAttributesComponent();
}

public class AddAttributesComponent : IPipelineComponent<ReflectionContext>
{
    public Task<Result> Process(PipelineContext<ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyAttributes)
        {
            return Task.FromResult(Result.Continue());
        }

        context.Request.Builder.AddAttributes(context.Request.SourceModel.GetCustomAttributes(true).ToAttributes(
            x => context.Request.MapAttribute(x.ConvertToDomainAttribute(context.Request.InitializeDelegate)),
            context.Request.Settings.CopyAttributes,
            context.Request.Settings.CopyAttributePredicate));

        return Task.FromResult(Result.Continue());
    }
}
