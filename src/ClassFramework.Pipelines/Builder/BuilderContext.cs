namespace ClassFramework.Pipelines.Builder;

public class BuilderContext : ContextBase<IType>
{
    public BuilderContext(IType sourceModel, PipelineSettings settings, IFormatProvider formatProvider)
        : base(sourceModel, settings, formatProvider)
    {
    }

    public IEnumerable<Property> GetSourceProperties()
        => SourceModel.Properties.Where(x => SourceModel.IsMemberValidForBuilderClass(x, Settings));

    public bool IsBuilderForAbstractEntity => Settings.EnableInheritance && (Settings.BaseClass is null || Settings.IsAbstract);
    public bool IsBuilderForOverrideEntity => Settings.EnableInheritance && Settings.BaseClass is not null;
    public bool IsAbstractBuilder => Settings.EnableBuilderInheritance && (Settings.BaseClass is null || Settings.IsAbstract) && !Settings.IsForAbstractBuilder;

    protected override string NewCollectionTypeName => Settings.BuilderNewCollectionTypeName;

    public string[] CreatePragmaWarningDisableStatementsForBuildMethod()
        => NeedsPragmasForBuildMethod()
            ?
            [
                "#pragma warning disable CS8604 // Possible null reference argument.",
                "#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.",
            ]
            : Array.Empty<string>();

    public string[] CreatePragmaWarningRestoreStatementsForBuildMethod()
        => NeedsPragmasForBuildMethod()
            ?
            [
                "#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.",
                "#pragma warning restore CS8604 // Possible null reference argument.",
            ]
            : Array.Empty<string>();

    public bool HasBackingFields()
        => !(IsAbstractBuilder || !Settings.AddNullChecks)
        || Settings.AddBackingFields
        || Settings.CreateAsObservable;

    private bool NeedsPragmasForBuildMethod()
        => Settings.EnableNullableReferenceTypes
        && !IsBuilderForAbstractEntity
        && !Settings.AddNullChecks;

    public bool IsValidForFluentMethod(Property property)
    {
        property = property.IsNotNull(nameof(property));

        if (!Settings.CopyInterfaces || !Settings.InheritFromInterfaces)
        {
            return true;
        }

        if (string.IsNullOrEmpty(property.ParentTypeFullName))
        {
            return true;
        }

        if (property.ParentTypeFullName == SourceModel.GetFullName())
        {
            return true;
        }

        var isInterfaced = SourceModel.Interfaces.Any(x => x == property.ParentTypeFullName);

        return !isInterfaced;
    }
}
