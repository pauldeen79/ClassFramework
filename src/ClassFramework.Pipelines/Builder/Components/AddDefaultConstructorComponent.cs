namespace ClassFramework.Pipelines.Builder.Components;

public class AddDefaultConstructorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<GenerateBuilderCommand, ClassBuilder>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ExecuteAsync(GenerateBuilderCommand context, ClassBuilder response, ICommandService commandService, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        response = response.IsNotNull(nameof(response));

        if (context.Settings.EnableBuilderInheritance
            && context.IsAbstractBuilder
            && !context.Settings.IsForAbstractBuilder)
        {
            response.AddConstructors(CreateInheritanceDefaultConstructor(context));
        }
        else
        {
            var defaultConstructorResult = await CreateDefaultConstructorAsync(context, response, token).ConfigureAwait(false);
            if (!defaultConstructorResult.IsSuccessful())
            {
                return defaultConstructorResult;
            }

            response.AddConstructors(defaultConstructorResult.Value!);
        }

        return Result.Success();
    }

    private async Task<Result<ConstructorBuilder>> CreateDefaultConstructorAsync(GenerateBuilderCommand context, ClassBuilder response, CancellationToken token)
    {
        var constructorInitializerResults = await GetConstructorInitializerResultsAsync(context).ConfigureAwait(false);

        var errorResult = constructorInitializerResults.Find(x => !x.Result.IsSuccessful());
        if (errorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(errorResult.Result);
        }

        var ctor = new ConstructorBuilder()
            .WithChainCall(await CreateBuilderClassConstructorChainCallAsync(context.SourceModel, context.Settings).ConfigureAwait(false))
            .WithProtected(context.IsBuilderForAbstractEntity)
            .AddCodeStatements(constructorInitializerResults.Select(x => $"{x.Name} = {x.Result.Value};"));

        if (context.Settings.SetDefaultValuesInEntityConstructor)
        {
            var defaultValueResults = await GetDefaultValueResultsAsync(context, token).ConfigureAwait(false);

            var defaultValueErrorResult = defaultValueResults.Find(x => !x.IsSuccessful());
            if (defaultValueErrorResult is not null)
            {
                return Result.FromExistingResult<ConstructorBuilder>(defaultValueErrorResult);
            }

            ctor.AddCodeStatements(defaultValueResults.Select(x => x.Value!.ToString()));

            var setDefaultValuesMethodNameResult = await _evaluator.EvaluateInterpolatedStringAsync(context.Settings.SetDefaultValuesMethodName, context.FormatProvider, context, token).ConfigureAwait(false);
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

    private async Task<List<Result<GenericFormattableString>>> GetDefaultValueResultsAsync(GenerateBuilderCommand context, CancellationToken token)
    {
        var defaultValueResults = new List<Result<GenericFormattableString>>();

        foreach (var property in context.GetSourceProperties()
            .Where
            (x => !x.TypeName.FixTypeName().IsCollectionTypeName()
               && ((!x.IsValueType && !x.IsNullable) || (x.Attributes.Any(y => y.Name == typeof(DefaultValueAttribute).FullName) && context.Settings.UseDefaultValueAttributeValuesForBuilderInitialization))
            ))
        {
            var result = await GenerateDefaultValueStatementAsync(property, context, token).ConfigureAwait(false);
            defaultValueResults.Add(result);
            if (!result.IsSuccessful())
            {
                break;
            }
        }

        return defaultValueResults;
    }

    private async Task<List<ConstructorInitializerItem>> GetConstructorInitializerResultsAsync(GenerateBuilderCommand context)
    {
        var constructorInitializerResults = new List<ConstructorInitializerItem>();

        foreach (var property in context.GetSourceProperties()
            .Where(x => x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var name = property.GetBuilderMemberName(context.Settings, context.FormatProvider.ToCultureInfo());
            var result = await property.GetBuilderConstructorInitializerAsync(
                context,
                new ParentChildContext<GenerateBuilderCommand, Property>(context, property, context.Settings),
                context.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName),
                context.Settings.BuilderNewCollectionTypeName,
                string.Empty,
                _evaluator).ConfigureAwait(false);

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

    private Task<Result<GenericFormattableString>> GenerateDefaultValueStatementAsync(Property property, GenerateBuilderCommand context, CancellationToken token)
        => _evaluator.EvaluateInterpolatedStringAsync
        (
            "{property.BuilderMemberName} = {property.DefaultValue};",
            context.FormatProvider,
            new ParentChildContext<GenerateBuilderCommand, Property>(context, property, context.Settings),
            token
        );

    private static ConstructorBuilder CreateInheritanceDefaultConstructor(GenerateBuilderCommand context)
        => new ConstructorBuilder()
            .WithChainCall("base()")
            .WithProtected(context.IsBuilderForAbstractEntity);

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
