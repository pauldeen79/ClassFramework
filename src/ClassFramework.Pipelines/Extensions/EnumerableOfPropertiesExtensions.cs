﻿namespace ClassFramework.Pipelines.Extensions;

public static class EnumerableOfPropertiesExtensions
{
    public static IEnumerable<ParameterBuilder> CreateImmutableClassCtorParameters(
        this IEnumerable<Property> properties,
        IFormatProvider formatProvider,
        Func<string, string> mapTypeNameDelegate)
        => properties
            .Select
            (
                property => new ParameterBuilder()
                    .WithName(property.Name.ToCamelCase(formatProvider.ToCultureInfo()))
                    .WithTypeName(mapTypeNameDelegate(property.TypeName).FixCollectionTypeName(typeof(IEnumerable<>).WithoutGenerics()))
                    .SetTypeContainerPropertiesFrom(property)
            );
}
