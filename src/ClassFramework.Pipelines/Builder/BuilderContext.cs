﻿namespace ClassFramework.Pipelines.Builder;

public class BuilderContext(TypeBase sourceModel, PipelineSettings settings, IFormatProvider formatProvider) : ContextBase<TypeBase>(sourceModel, settings, formatProvider)
{
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
            : [];

    public string[] CreatePragmaWarningRestoreStatementsForBuildMethod()
        => NeedsPragmasForBuildMethod()
            ?
            [
                "#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.",
                "#pragma warning restore CS8604 // Possible null reference argument.",
            ]
            : [];

    public bool HasBackingFields()
        => !(IsAbstractBuilder || !Settings.AddNullChecks)
        || Settings.AddBackingFields
        || Settings.CreateAsObservable;

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

    public string ReturnValueStatementForFluentMethod => $"return {ReturnValue};";

    public string ReturnType
    {
        get
        {
            if (IsBuilderForAbstractEntity || IsBuilderForOverrideEntity)
            {
                return MapTypeName(SourceModel.GetFullName(), string.Empty);
            }

            return Settings.InheritFromInterfaces
                ? MapTypeName(SourceModel.Interfaces.FirstOrDefault(x => x.GetClassName() == $"I{SourceModel.Name}") ?? SourceModel.GetFullName(), string.Empty)
                : MapTypeName(SourceModel.GetFullName(), string.Empty);
        }
    }

    public Result<T>[] GetInterfaceResults<T>(
        Func<string, GenericFormattableString, T> namespaceTransformation,
        Func<string, T> noNamespaceTransformation,
        IFormattableStringParser formattableStringParser,
        bool includeNonAbstractionNamespaces)
        => SourceModel.Interfaces
            .Where(x => (Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
                        && (includeNonAbstractionNamespaces || Settings.BuilderAbstractionsTypeConversionNamespaces.Contains(x.GetNamespaceWithDefault())))
            .Select(x =>
            {
                var metadata = GetMappingMetadata(x).ToArray();
                var ns = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceNamespace);

                if (!string.IsNullOrEmpty(ns))
                {
                    var property = new PropertyBuilder()
                        .WithName("Dummy")
                        .WithTypeName(x)
                        .Build();
                    var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceName, "{NoGenerics(ClassName($property.TypeName))}Builder{GenericArguments($property.TypeName, true)}");
                    var newFullName = $"{ns}.{newTypeName}";

                    return formattableStringParser.Parse
                    (
                        newFullName,
                        FormatProvider,
                        new ParentChildContext<PipelineContext<BuilderContext>, Property>(new PipelineContext<BuilderContext>(this), property, Settings)
                    ).Transform(y => namespaceTransformation(x, y));
                }
                return Result.Success(noNamespaceTransformation(x));
            })
            .TakeWhileWithFirstNonMatching(x => x.IsSuccessful())
            .ToArray();

    private string ReturnValue
    {
        get
        {
            if (IsBuilderForAbstractEntity)
            {
                return "(TBuilder)this";
            }

            return "this";
        }
    }

    private bool NeedsPragmasForBuildMethod()
        => Settings.EnableNullableReferenceTypes
        && !IsBuilderForAbstractEntity
        && !Settings.AddNullChecks;

    public ClassBuilder Builder => _wrappedBuilder.Builder;

    private readonly ClassBuilderWrapper _wrappedBuilder = new();
}
