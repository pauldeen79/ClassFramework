
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

        if (!context.Context.Settings.CopyAttributes)
        {
            return Task.FromResult(Result.Continue<TypeBaseBuilder>());
        }

        context.Model.AddAttributes(context.Context.SourceModel.GetCustomAttributes(true).ToAttributes(
            x => context.Context.MapAttribute(x.ConvertToDomainAttribute(context.Context.InitializeDelegate)),
            context.Context.Settings.CopyAttributes,
            context.Context.Settings.CopyAttributePredicate));

        return Task.FromResult(Result.Continue<TypeBaseBuilder>());
    }
}
