namespace ClassFramework.Pipelines.Interface.Components;

public class ValidationComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceContext, InterfaceBuilder> Build() => new ValidationComponent();
}

public class ValidationComponent : IPipelineComponent<InterfaceContext, InterfaceBuilder>
{
    public Task<Result> Process(PipelineContext<InterfaceContext, InterfaceBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AllowGenerationWithoutProperties
            && context.Request.SourceModel.Properties.Count == 0)
        {
            return Task.FromResult(Result.Invalid("To create an interface, there must be at least one property"));
        }
        
        return Task.FromResult(Result.Continue());
    }
}
