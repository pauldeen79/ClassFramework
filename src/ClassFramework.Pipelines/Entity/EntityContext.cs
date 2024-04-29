﻿namespace ClassFramework.Pipelines.Entity;

public class EntityContext : ContextBase<TypeBase, IConcreteType>
{
    public EntityContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider)
        : base(sourceModel, settings, formatProvider)
    {
    }

    public bool IsAbstract
        => Settings.EnableInheritance
        && Settings.IsAbstract;

    protected override string NewCollectionTypeName => Settings.EntityNewCollectionTypeName;

    protected override IBuilder<IConcreteType> CreateResponseBuilder() => new ClassBuilderWrapper();
}
