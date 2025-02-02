namespace ClassFramework.Pipelines.Interface.Components;

public class AddBuilderAbstractionsTypeConversionComponent : IPipelineComponent<InterfaceContext>
{
    public Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.UseBuilderAbstractionsTypeConversion)
        {
            return Task.FromResult(Result.Success());
        }

        // TODO; Add Build or ToBuilder methods for each interface.
        // Check wether the interface is an entity (.Abstractions) or a builder (.Builder.Abstractions)
        return Task.FromResult(Result.Success());
    }
}
