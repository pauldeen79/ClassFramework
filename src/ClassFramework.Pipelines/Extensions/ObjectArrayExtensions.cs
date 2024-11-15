namespace ClassFramework.Pipelines.Extensions;

public static class ObjectArrayExtensions
{
    public static IEnumerable<AttributeBuilder> ToAttributes(
        this object[] attributes,
        Func<System.Attribute, Domain.Attribute> mapAttributeDelegate,
        bool copyAttributesEnabled,
        Predicate<Domain.Attribute>? attributeFilterPredicate)
    {
        mapAttributeDelegate = mapAttributeDelegate.IsNotNull(nameof(mapAttributeDelegate));

        if (!copyAttributesEnabled)
        {
            return [];
        }

        return attributes
            .OfType<System.Attribute>()
            .Where(x => x.GetType().FullName != "System.Runtime.CompilerServices.NullableContextAttribute"
                     && x.GetType().FullName != "System.Runtime.CompilerServices.NullableAttribute")
            .Select(mapAttributeDelegate)
            .Where(x => attributeFilterPredicate?.Invoke(x) ?? true)
            .Select(x => x.ToBuilder());
    }
}
