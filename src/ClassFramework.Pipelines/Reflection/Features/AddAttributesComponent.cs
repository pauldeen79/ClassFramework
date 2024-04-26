
namespace ClassFramework.Pipelines.Reflection.Features;

public class AddAttributesComponentBuilder : IReflectionComponentBuilder
{
    public IPipelineComponent<TypeBaseBuilder, ReflectionContext> Build()
        => new AddAttributesComponent();
}

public class AddAttributesComponent : IPipelineComponent<TypeBaseBuilder, ReflectionContext>
{
    public Task<Result<TypeBaseBuilder>> Process(PipelineContext<TypeBaseBuilder, ReflectionContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyAttributes)
        {
            return Task.FromResult(Result.Continue<TypeBaseBuilder>());
        }

        context.Response.AddAttributes(context.Request.SourceModel.GetCustomAttributes(true).ToAttributes(
            x => context.Request.MapAttribute(x.ConvertToDomainAttribute(context.Request.InitializeDelegate)),
            context.Request.Settings.CopyAttributes,
            context.Request.Settings.CopyAttributePredicate));

        return Task.FromResult(Result.Continue<TypeBaseBuilder>());
    }
}
