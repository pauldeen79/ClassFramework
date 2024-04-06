namespace ClassFramework.Pipelines.Interface.Features;

public class AddAttributesComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceBuilder, InterfaceContext> Build()
        => new AddAttributesComponent();
}

public class AddAttributesComponent : IPipelineComponent<InterfaceBuilder, InterfaceContext>
{
    public Result<InterfaceBuilder> Process(PipelineContext<InterfaceBuilder, InterfaceContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Context.Settings.CopyAttributes)
        {
            return Result.Continue<InterfaceBuilder>();
        }

        context.Model.AddAttributes(context.Context.SourceModel.Attributes
            .Where(x => context.Context.Settings.CopyAttributePredicate?.Invoke(x) ?? true)
            .Select(x => context.Context.MapAttribute(x).ToBuilder()));

        return Result.Continue<InterfaceBuilder>();
    }
}
