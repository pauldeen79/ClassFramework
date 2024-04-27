namespace ClassFramework.Pipelines.Interface.Components;

public class AddPropertiesComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceContext, InterfaceBuilder> Build()
        => new AddPropertiesComponent();
}

public class AddPropertiesComponent : IPipelineComponent<InterfaceContext, InterfaceBuilder>
{
    public Task<Result> Process(PipelineContext<InterfaceContext, InterfaceBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var properties = context.Request.SourceModel
            .Properties
            .Where(property => context.Request.SourceModel.IsMemberValidForBuilderClass(property, context.Request.Settings))
            .ToArray();

        context.Response.AddProperties
        (
            properties.Select
            (
                property => context.Request.CreatePropertyForEntity(property)
                    .WithHasGetter(property.HasGetter)
                    .WithHasInitializer(false)
                    .WithHasSetter(property.HasSetter && context.Request.Settings.AddSetters)
            )
        );

        return Task.FromResult(Result.Continue());
    }
}
