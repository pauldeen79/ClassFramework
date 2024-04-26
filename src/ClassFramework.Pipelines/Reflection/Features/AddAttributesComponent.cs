
namespace ClassFramework.Pipelines.Reflection.Features;

public class AddAttributesComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<ReflectionContext, TypeBaseBuilder> Build()
        => new AddAttributesComponent();
}

public class AddAttributesComponent : IPipelineComponent<ReflectionContext, TypeBaseBuilder>
{
    public Task<Result> Process(PipelineContext<ReflectionContext, TypeBaseBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyAttributes)
        {
            return Task.FromResult(Result.Continue());
        }

        context.Response.AddAttributes(context.Request.SourceModel.GetCustomAttributes(true).ToAttributes(
            x => context.Request.MapAttribute(x.ConvertToDomainAttribute(context.Request.InitializeDelegate)),
            context.Request.Settings.CopyAttributes,
            context.Request.Settings.CopyAttributePredicate));

        return Task.FromResult(Result.Continue());
    }
}
