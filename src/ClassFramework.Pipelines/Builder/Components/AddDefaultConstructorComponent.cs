namespace ClassFramework.Pipelines.Builder.Components;

public class AddDefaultConstructorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand command, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        command = command.IsNotNull(nameof(command));
        response = response.IsNotNull(nameof(response));

        if (command.Settings.EnableBuilderInheritance
            && command.IsAbstractBuilder
            && !command.Settings.IsForAbstractBuilder)
        {
            response.AddConstructors(CreateInheritanceDefaultConstructor(command));
        }
        else
        {
            var defaultConstructorResult = await CreateDefaultConstructorAsync(command, response, token).ConfigureAwait(false);
            if (!defaultConstructorResult.IsSuccessful())
            {
                return defaultConstructorResult;
            }

            response.AddConstructors(defaultConstructorResult.Value!);
        }

        return Result.Success();
    }

    private async Task<Result<ConstructorBuilder>> CreateDefaultConstructorAsync(GenerateBuilderCommand command, ClassBuilder response, CancellationToken token)
    {
        var constructorInitializerResults = await GetConstructorInitializerResultsAsync(command, token).ConfigureAwait(false);

        var errorResult = constructorInitializerResults.Find(x => !x.Result.IsSuccessful());
        if (errorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(errorResult.Result);
        }

        var ctor = new ConstructorBuilder()
            .WithChainCall(await CreateBuilderClassConstructorChainCallAsync(command.SourceModel, command.Settings).ConfigureAwait(false))
            .WithProtected(command.IsBuilderForAbstractEntity)
            .AddCodeStatements(constructorInitializerResults.Select(x => $"{x.Name} = {x.Result.Value};"));

        if (command.Settings.SetDefaultValuesInEntityConstructor)
        {
            var defaultValueResults = await GetDefaultValueResultsAsync(command, token).ConfigureAwait(false);

            var defaultValueErrorResult = defaultValueResults.Find(x => !x.IsSuccessful());
            if (defaultValueErrorResult is not null)
            {
                return Result.FromExistingResult<ConstructorBuilder>(defaultValueErrorResult);
            }

            ctor.AddCodeStatements(defaultValueResults.Select(x => x.Value!.ToString()));

            var setDefaultValuesMethodNameResult = await _evaluator.EvaluateInterpolatedStringAsync(command.Settings.SetDefaultValuesMethodName, command.FormatProvider, command, token).ConfigureAwait(false);
            if (!setDefaultValuesMethodNameResult.IsSuccessful())
            {
                return Result.FromExistingResult<ConstructorBuilder>(setDefaultValuesMethodNameResult);
            }

            if (!string.IsNullOrEmpty(setDefaultValuesMethodNameResult.Value!.ToString()))
            {
                ctor.AddCodeStatements($"{setDefaultValuesMethodNameResult.Value}();");
                response.AddMethods(new MethodBuilder()
                    .WithName(setDefaultValuesMethodNameResult.Value)
                    .WithPartial()
                    .WithVisibility(Visibility.Private)
                    );
            }
        }

        return Result.Success(ctor);
    }

    private async Task<List<Result<GenericFormattableString>>> GetDefaultValueResultsAsync(GenerateBuilderCommand command, CancellationToken token)
    {
        var defaultValueResults = new List<Result<GenericFormattableString>>();

        foreach (var property in command.GetSourceProperties()
            .Where
            (x => !x.TypeName.FixTypeName().IsCollectionTypeName()
               && ((!x.IsValueType && !x.IsNullable) || (x.Attributes.Any(y => y.Name == typeof(DefaultValueAttribute).FullName) && command.Settings.UseDefaultValueAttributeValuesForBuilderInitialization))
            ))
        {
            var result = await GenerateDefaultValueStatementAsync(property, command, token).ConfigureAwait(false);
            defaultValueResults.Add(result);
            if (!result.IsSuccessful())
            {
                break;
            }
        }

        return defaultValueResults;
    }

    private async Task<List<ConstructorInitializerItem>> GetConstructorInitializerResultsAsync(GenerateBuilderCommand command,CancellationToken cancellationToken)
    {
        var constructorInitializerResults = new List<ConstructorInitializerItem>();

        foreach (var property in command.GetSourceProperties()
            .Where(x => x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var name = property.GetBuilderMemberName(command.Settings, command.FormatProvider.ToCultureInfo());
            var result = await property.GetBuilderConstructorInitializerAsync(
                command,
                new ParentChildContext<GenerateBuilderCommand, Property>(command, property, command.Settings),
                command.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName),
                string.Empty,
                _evaluator,
                cancellationToken).ConfigureAwait(false);

            constructorInitializerResults.Add(new ConstructorInitializerItem(name, result));

            if (!result.IsSuccessful())
            {
                break;
            }
        }

        return constructorInitializerResults;
    }

    private static async Task<string> CreateBuilderClassConstructorChainCallAsync(IType instance, PipelineSettings settings)
        => (await instance.GetCustomValueForInheritedClassAsync(settings.EnableInheritance, _ => Task.FromResult(Result.Success<GenericFormattableString>("base()")))
            .ConfigureAwait(false)).Value!; //note that the delegate always returns success, so we can simply use the Value here

    private Task<Result<GenericFormattableString>> GenerateDefaultValueStatementAsync(Property property, GenerateBuilderCommand command, CancellationToken token)
        => _evaluator.EvaluateInterpolatedStringAsync
        (
            "{property.BuilderMemberName} = {property.DefaultValue};",
            command.FormatProvider,
            new ParentChildContext<GenerateBuilderCommand, Property>(command, property, command.Settings),
            token
        );

    private static ConstructorBuilder CreateInheritanceDefaultConstructor(GenerateBuilderCommand command)
        => new ConstructorBuilder()
            .WithChainCall("base()")
            .WithProtected(command.IsBuilderForAbstractEntity);

    private sealed class ConstructorInitializerItem
    {
        public ConstructorInitializerItem(string name, Result<GenericFormattableString> result)
        {
            Name = name;
            Result = result;
        }

        public string Name { get; }
        public Result<GenericFormattableString> Result { get; }
    }
}
