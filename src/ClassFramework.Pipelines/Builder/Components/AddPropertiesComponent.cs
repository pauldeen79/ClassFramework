namespace ClassFramework.Pipelines.Builder.Components;

public class AddPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        if (command.IsAbstractBuilder)
        {
            return Result.Continue();
        }

        foreach (var property in command.GetSourceProperties())
        {
            var results = await new AsyncResultDictionaryBuilder<GenericFormattableString>()
                .Add(ResultNames.TypeName, () => property.GetBuilderArgumentTypeNameAsync(command, new ParentChildContext<GenerateBuilderCommand, Property>(command, property, command.Settings), command.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _evaluator, token))
                .Add(ResultNames.ParentTypeName, () => property.GetBuilderParentTypeNameAsync(command, _evaluator, token))
                .BuildAsync(token)
                .ConfigureAwait(false);

            var error = results.GetError();
            if (error is not null)
            {
                // Error in formattable string parsing
                return error;
            }

            var propertyBuilder = new PropertyBuilder()
                .WithName(property.Name)
                .WithTypeName(results.GetValue(ResultNames.TypeName).ToString()
                    .FixCollectionTypeName(command.Settings.BuilderNewCollectionTypeName)
                    .FixNullableTypeName(property))
                .WithIsNullable(property.IsNullable)
                .WithIsValueType(property.IsValueType)
                .AddGenericTypeArguments(property.GenericTypeArguments.Select(x => x.ToBuilder()))
                .WithParentTypeFullName(results.GetValue(ResultNames.ParentTypeName))
                .AddAttributes(property.Attributes
                    .Where(_ => command.Settings.CopyAttributes)
                    .Select(x => command.MapAttribute(x).ToBuilder()))
                .AddGetterCodeStatements(CreateBuilderPropertyGetterStatements(property, command))
                .AddSetterCodeStatements(await CreateBuilderPropertySetterStatementsAsync(property, command, token).ConfigureAwait(false));

            if (!command.Settings.FluentBuilderMethods)
            {
                response.AddProperties(propertyBuilder);
            }
            else
            {
                response.AddProperties(propertyBuilder.Build().ToBuilder()
                    .With(p => p.SetterCodeStatements.OfType<StringCodeStatementBuilder>().ToList().ForEach(s => s.Statement = s.Statement.Replace($"nameof({p.Name})", $"nameof({p.Name}Property)")))
                    .WithName(propertyBuilder.Name + "Property"));
                response.AddMethods(command.ConvertPropertyToMethods(
                    propertyBuilder,
                    results.GetValue(ResultNames.TypeName).ToString()
                        .FixCollectionTypeName(command.Settings.BuilderNewCollectionTypeName)
                        .FixNullableTypeName(property),
                    property.IsNullable,
                    property.IsValueType));
            }
        }

        // Note that we are not checking the result, because the same formattable string (CustomBuilderArgumentType) has already been checked earlier in this class
        // We can simple use Value with bang operator to keep the compiler happy (the value should be a string, and not be null)
        response.AddFields((await command.SourceModel
            .GetBuilderClassFieldsAsync(command, _evaluator, token).ConfigureAwait(false))
            .Select(x => x.Value!));

        return Result.Success();
    }

    private static IEnumerable<CodeStatementBaseBuilder> CreateBuilderPropertyGetterStatements(Property property, GenerateBuilderCommand command)
    {
        if (property.HasBackingFieldOnBuilder(command.Settings))
        {
            yield return new StringCodeStatementBuilder($"return _{property.Name.ToCamelCase(command.FormatProvider.ToCultureInfo())};");
        }
    }

    private async Task<IEnumerable<CodeStatementBaseBuilder>> CreateBuilderPropertySetterStatementsAsync(Property property, GenerateBuilderCommand command, CancellationToken token)
    {
        var results = new List<CodeStatementBaseBuilder>();
        if (property.HasBackingFieldOnBuilder(command.Settings))
        {
            if (command.Settings.CreateAsObservable)
            {
                var nullSuffix = command.Settings.EnableNullableReferenceTypes && !property.IsValueType
                    ? "!"
                    : string.Empty;
                results.Add(new StringCodeStatementBuilder($"bool hasChanged = !{typeof(EqualityComparer<>).WithoutGenerics()}<{(await property.GetBuilderArgumentTypeNameAsync(command, new ParentChildContext<GenerateBuilderCommand, Property>(command, property, command.Settings), command.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), _evaluator, token).ConfigureAwait(false)).Value}>.Default.Equals(_{property.Name.ToCamelCase(command.FormatProvider.ToCultureInfo())}{nullSuffix}, value{nullSuffix});"));
            }

            results.Add(new StringCodeStatementBuilder($"_{property.Name.ToCamelCase(command.FormatProvider.ToCultureInfo())} = value{property.GetNullCheckSuffix("value", command.Settings.AddNullChecks, command.SourceModel)};"));

            if (command.Settings.CreateAsObservable)
            {
                results.Add(new StringCodeStatementBuilder($"if (hasChanged) HandlePropertyChanged(nameof({property.Name}));"));
            }
        }

        return results;
    }
}
