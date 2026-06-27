namespace ClassFramework.Pipelines.Interface.Components;

public class AddPropertiesComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateInterfaceCommand command, InterfaceBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        return (await new AsyncResultDictionaryBuilder<GenericFormattableString>()
            .Add(ResultNames.Name, () => _evaluator.EvaluateInterpolatedStringAsync(command.Settings.NameFormatString, command.FormatProvider, command, token))
            .BuildAsync(token)
            .ConfigureAwait(false))
            .OnSuccess(results =>
            {
                var properties = command.GetSourceProperties().Select
                (
                    property => command.CreatePropertyForEntity(property, command.Settings.BuilderAbstractionsTypeConversionMetadataName)
                        .WithHasGetter(property.HasGetter)
                        .WithHasInitializer(false)
                        .WithHasSetter(property.HasSetter && command.Settings.AddSetters)
                );

                if (command.Settings.AddProperties)
                {
                    response.AddProperties(properties);
                }
                else
                {
                    response.AddMethods(properties.SelectMany(property => command.ConvertPropertyToMethods(
                        property,
                        property.TypeName,
                        property.IsNullable,
                        property.IsValueType,
                        string.Empty /*property.ParentTypeFullName.WhenNullOrEmpty(() => results.GetValue(ResultNames.Name))*/)));
                }
            });
    }
}
