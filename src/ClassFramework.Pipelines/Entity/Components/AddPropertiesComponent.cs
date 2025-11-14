namespace ClassFramework.Pipelines.Entity.Components;

public class AddPropertiesComponent : IPipelineComponent<GenerateEntityCommand, ClassBuilder>
{
    public Task<Result> ExecuteAsync(GenerateEntityCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
        => Task.Run(() =>
        {
            command = command.IsNotNull(nameof(command));
            response = response.IsNotNull(nameof(response));

            var properties = command.GetSourceProperties().ToArray();

            response.AddProperties(
                properties.Select
                (
                    property => command.CreatePropertyForEntity(property)
                        .WithVirtual(property.Virtual)
                        .WithAbstract(property.Abstract)
                        .WithProtected(property.Protected)
                        .WithOverride(property.Override)
                        .WithHasInitializer(property.HasInitializer && !(command.Settings.AddSetters || command.Settings.AddBackingFields || command.Settings.CreateAsObservable))
                        .WithHasSetter(((command.Settings.AddSetters || command.Settings.AddBackingFields) && !property.TypeName.IsCollectionTypeName()) || command.Settings.CreateAsObservable)
                        .WithGetterVisibility(property.GetterVisibility)
                        .WithSetterVisibility(command.Settings.SetterVisibility)
                        .WithInitializerVisibility(property.InitializerVisibility)
                        .WithExplicitInterfaceName(property.ExplicitInterfaceName)
                        .WithParentTypeFullName(property.ParentTypeFullName)
                        .AddGetterCodeStatements(CreateBuilderPropertyGetterStatements(property, command))
                        .AddSetterCodeStatements(CreateBuilderPropertySetterStatements(property, command))
                )
            );

            if (command.Settings.AddBackingFields || command.Settings.CreateAsObservable)
            {
                AddBackingFields(command, response, properties);
            }

            return Result.Success();
        }, token);

    private static void AddBackingFields(GenerateEntityCommand command, ClassBuilder response, Property[] properties)
        => response.AddFields
        (
            properties
                .Select
                (
                    property => new FieldBuilder()
                        .WithName($"_{property.Name.ToCamelCase(command.FormatProvider.ToCultureInfo())}")
                        .WithTypeName(command.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName)
                            .FixCollectionTypeName(command.Settings.CollectionTypeName
                                .WhenNullOrEmpty(command.Settings.EntityNewCollectionTypeName)
                                .WhenNullOrEmpty(typeof(List<>).WithoutGenerics()))
                        .FixNullableTypeName(property))
                        .WithIsNullable(property.IsNullable)
                        .WithIsValueType(property.IsValueType)
                        .AddGenericTypeArguments(property.GenericTypeArguments.Select(x => x.ToBuilder()))
                )
        );

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertyGetterStatements(Property property, GenerateEntityCommand command)
    {
        if (command.Settings.AddBackingFields || command.Settings.CreateAsObservable)
        {
            yield return new StringCodeStatementBuilder($"return _{property.Name.ToCamelCase(command.FormatProvider.ToCultureInfo())};");
        }
    }

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertySetterStatements(Property property, GenerateEntityCommand command)
    {
        if (command.Settings.AddBackingFields || command.Settings.CreateAsObservable)
        {
            if (command.Settings.CreateAsObservable)
            {
                var nullSuffix = command.Settings.EnableNullableReferenceTypes && !property.IsValueType
                    ? "!"
                    : string.Empty;
                yield return new StringCodeStatementBuilder($"bool hasChanged = !{typeof(EqualityComparer<>).WithoutGenerics()}<{command.CreatePropertyForEntity(property).TypeName}>.Default.Equals(_{property.Name.ToCamelCase(command.FormatProvider.ToCultureInfo())}{nullSuffix}, value{nullSuffix});");
            }

            yield return new StringCodeStatementBuilder($"_{property.Name.ToCamelCase(command.FormatProvider.ToCultureInfo())} = value{property.GetNullCheckSuffix("value", command.Settings.AddNullChecks, command.SourceModel)};");

            if (command.Settings.CreateAsObservable)
            {
                yield return new StringCodeStatementBuilder($"if (hasChanged) HandlePropertyChanged(nameof({property.Name}));");
            }
        }
    }
}
