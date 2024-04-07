﻿namespace ClassFramework.Pipelines.Entity.Features;

public class AbstractEntityComponentBuilder : IEntityComponentBuilder
{
    public IPipelineComponent<IConcreteTypeBuilder, EntityContext> Build()
        => new AbstractEntityComponent();
}

public class AbstractEntityComponent : IPipelineComponent<IConcreteTypeBuilder, EntityContext>
{
    public Result<IConcreteTypeBuilder> Process(PipelineContext<IConcreteTypeBuilder, EntityContext> context)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Model is ClassBuilder cls)
        {
            cls.WithAbstract(context.Context.IsAbstract);
        }

        return Result.Continue<IConcreteTypeBuilder>();
    }
}