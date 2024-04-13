﻿namespace ClassFramework.Pipelines.Interface.Features;

public class AddPropertiesComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceBuilder, InterfaceContext> Build()
        => new AddPropertiesComponent();
}

public class AddPropertiesComponent : IPipelineComponent<InterfaceBuilder, InterfaceContext>
{
    public Task<Result<InterfaceBuilder>> Process(PipelineContext<InterfaceBuilder, InterfaceContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        var properties = context.Context.SourceModel
            .Properties
            .Where(property => context.Context.SourceModel.IsMemberValidForBuilderClass(property, context.Context.Settings))
            .ToArray();

        context.Model.AddProperties
        (
            properties.Select
            (
                property => context.Context.CreatePropertyForEntity(property)
                    .WithHasGetter(property.HasGetter)
                    .WithHasInitializer(false)
                    .WithHasSetter(property.HasSetter && context.Context.Settings.AddSetters)
            )
        );

        return Task.FromResult(Result.Continue<InterfaceBuilder>());
    }
}
