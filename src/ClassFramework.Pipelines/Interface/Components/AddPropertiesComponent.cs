namespace ClassFramework.Pipelines.Interface.Components;

public class AddPropertiesComponent : IPipelineComponent<InterfaceContext>
{
    public Task<Result> ProcessAsync(PipelineContext<InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var properties = context.Request.SourceModel
            .Properties
            .Where(property => context.Request.SourceModel.IsMemberValidForBuilderClass(property, context.Request.Settings))
            .ToArray();

        string metadataName = string.Empty;
        if (context.Request.Settings.UseBuilderAbstractionsTypeConversion)
        {
            // TODO: Maybe add a setting so we can safely determine whether we are creating a Builder or an Entity interface...
            metadataName = context.Request.SourceModel.Namespace.Contains("Builders") || context.Request.SourceModel.Name.Contains("Builder")
                ? MetadataNames.CustomBuilderInterfaceTypeName
                : string.Empty;
        }

        context.Request.Builder.AddProperties
        (
            properties.Select
            (
                property => context.Request.CreatePropertyForEntity(property, metadataName)
                    .WithHasGetter(property.HasGetter)
                    .WithHasInitializer(false)
                    .WithHasSetter(property.HasSetter && context.Request.Settings.AddSetters)
            )
        );

        return Task.FromResult(Result.Success());
    }
}
