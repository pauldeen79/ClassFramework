﻿namespace ClassFramework.Pipelines.BuilderExtension;

public class BuilderExtensionContext : ContextBase<TypeBase>

{
    public BuilderExtensionContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider)
        : base(sourceModel, settings, formatProvider)
    {
    }

    protected override string NewCollectionTypeName => Settings.BuilderNewCollectionTypeName;

    public IEnumerable<Property> GetSourceProperties()
        => SourceModel.Properties.Where(x => SourceModel.IsMemberValidForBuilderClass(x, Settings));

    public ClassBuilder Builder => _wrappedBuilder.Builder;

    private readonly ClassBuilderWrapper _wrappedBuilder = new();
}
