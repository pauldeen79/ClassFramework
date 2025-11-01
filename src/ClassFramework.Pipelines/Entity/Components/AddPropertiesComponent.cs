﻿namespace ClassFramework.Pipelines.Entity.Components;

public class AddPropertiesComponent : IPipelineComponent<EntityContext>
{
    public Task<Result> ExecuteAsync(EntityContext context, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            context = context.IsNotNull(nameof(context));

            var properties = context.GetSourceProperties().ToArray();

            context.Builder.AddProperties(
                properties.Select
                (
                    property => context.CreatePropertyForEntity(property)
                        .WithVirtual(property.Virtual)
                        .WithAbstract(property.Abstract)
                        .WithProtected(property.Protected)
                        .WithOverride(property.Override)
                        .WithHasInitializer(property.HasInitializer && !(context.Settings.AddSetters || context.Settings.AddBackingFields || context.Settings.CreateAsObservable))
                        .WithHasSetter(((context.Settings.AddSetters || context.Settings.AddBackingFields) && !property.TypeName.IsCollectionTypeName()) || context.Settings.CreateAsObservable)
                        .WithGetterVisibility(property.GetterVisibility)
                        .WithSetterVisibility(context.Settings.SetterVisibility)
                        .WithInitializerVisibility(property.InitializerVisibility)
                        .WithExplicitInterfaceName(property.ExplicitInterfaceName)
                        .WithParentTypeFullName(property.ParentTypeFullName)
                        .AddGetterCodeStatements(CreateBuilderPropertyGetterStatements(property, context))
                        .AddSetterCodeStatements(CreateBuilderPropertySetterStatements(property, context))
                )
            );

            if (context.Settings.AddBackingFields || context.Settings.CreateAsObservable)
            {
                AddBackingFields(context, properties);
            }

            return Result.Success();
        }, token);

    private static void AddBackingFields(EntityContext context, Property[] properties)
        => context.Builder.AddFields
        (
            properties
                .Select
                (
                    property => new FieldBuilder()
                        .WithName($"_{property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo())}")
                        .WithTypeName(context.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName)
                            .FixCollectionTypeName(context.Settings.CollectionTypeName
                                .WhenNullOrEmpty(context.Settings.EntityNewCollectionTypeName)
                                .WhenNullOrEmpty(typeof(List<>).WithoutGenerics()))
                        .FixNullableTypeName(property))
                        .WithIsNullable(property.IsNullable)
                        .WithIsValueType(property.IsValueType)
                        .AddGenericTypeArguments(property.GenericTypeArguments.Select(x => x.ToBuilder()))
                )
        );

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertyGetterStatements(Property property, EntityContext context)
    {
        if (context.Settings.AddBackingFields || context.Settings.CreateAsObservable)
        {
            yield return new StringCodeStatementBuilder($"return _{property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo())};");
        }
    }

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertySetterStatements(Property property, EntityContext context)
    {
        if (context.Settings.AddBackingFields || context.Settings.CreateAsObservable)
        {
            if (context.Settings.CreateAsObservable)
            {
                var nullSuffix = context.Settings.EnableNullableReferenceTypes && !property.IsValueType
                    ? "!"
                    : string.Empty;
                yield return new StringCodeStatementBuilder($"bool hasChanged = !{typeof(EqualityComparer<>).WithoutGenerics()}<{context.CreatePropertyForEntity(property).TypeName}>.Default.Equals(_{property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo())}{nullSuffix}, value{nullSuffix});");
            }

            yield return new StringCodeStatementBuilder($"_{property.Name.ToCamelCase(context.FormatProvider.ToCultureInfo())} = value{property.GetNullCheckSuffix("value", context.Settings.AddNullChecks, context.SourceModel)};");

            if (context.Settings.CreateAsObservable)
            {
                yield return new StringCodeStatementBuilder($"if (hasChanged) HandlePropertyChanged(nameof({property.Name}));");
            }
        }
    }
}
