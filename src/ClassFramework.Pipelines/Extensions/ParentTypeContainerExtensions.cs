namespace ClassFramework.Pipelines.Extensions;

public static class ParentTypeContainerExtensions
{
    public static bool IsDefinedOn(
        this IParentTypeContainer instance,
        IType typeBase,
        InheritanceComparisonDelegate? comparisonDelegate = null)
    {
        typeBase = typeBase.IsNotNull(nameof(typeBase));

        return comparisonDelegate is null
            ? string.IsNullOrEmpty(instance.ParentTypeFullName) || instance.ParentTypeFullName == typeBase.GetFullName()
            : comparisonDelegate.Invoke(instance, typeBase);
    }
}
