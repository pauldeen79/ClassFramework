namespace ClassFramework.Pipelines.Interface.Features;

public class ValidationComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceBuilder, InterfaceContext> Build() => new ValidationComponent();
}

public class ValidationComponent : IPipelineComponent<InterfaceBuilder, InterfaceContext>
{
    public Result<InterfaceBuilder> Process(PipelineContext<InterfaceBuilder, InterfaceContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.AllowGenerationWithoutProperties
            && context.Context.SourceModel.Properties.Count == 0)
        {
            return Result.Invalid<InterfaceBuilder>("To create an interface, there must be at least one property");
        }
        
        return Result.Continue<InterfaceBuilder>();
    }
}
