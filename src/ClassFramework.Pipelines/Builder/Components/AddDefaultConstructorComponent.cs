namespace ClassFramework.Pipelines.Builder.Components;

public class AddDefaultConstructorComponent(IExpressionEvaluator evaluator) : IPipelineComponent<BuilderContext>
{
    private readonly IExpressionEvaluator _evaluator = evaluator.IsNotNull(nameof(evaluator));

    public async Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (context.Request.Settings.EnableBuilderInheritance
            && context.Request.IsAbstractBuilder
            && !context.Request.Settings.IsForAbstractBuilder)
        {
            context.Request.Builder.AddConstructors(CreateInheritanceDefaultConstructor(context));
        }
        else
        {
            var defaultConstructorResult = await CreateDefaultConstructorAsync(context, token).ConfigureAwait(false);
            if (!defaultConstructorResult.IsSuccessful())
            {
                return defaultConstructorResult;
            }

            context.Request.Builder.AddConstructors(defaultConstructorResult.Value!);
        }

        return Result.Success();
    }

    private async Task<Result<ConstructorBuilder>> CreateDefaultConstructorAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        var constructorInitializerResults = await GetConstructorInitializerResultsAsync(context).ConfigureAwait(false);

        var errorResult = constructorInitializerResults.Find(x => !x.Item2.IsSuccessful());
        if (errorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(errorResult.Item2);
        }

        var ctor = new ConstructorBuilder()
            .WithChainCall(await CreateBuilderClassConstructorChainCallAsync(context.Request.SourceModel, context.Request.Settings).ConfigureAwait(false))
            .WithProtected(context.Request.IsBuilderForAbstractEntity)
            .AddCodeStatements(constructorInitializerResults.Select(x => $"{x.Item1} = {x.Item2.Value};"));

        if (context.Request.Settings.SetDefaultValuesInEntityConstructor)
        {
            var defaultValueResults = await GetDefaultValueResultsAsync(context, token).ConfigureAwait(false);

            var defaultValueErrorResult = defaultValueResults.Find(x => !x.IsSuccessful());
            if (defaultValueErrorResult is not null)
            {
                return Result.FromExistingResult<ConstructorBuilder>(defaultValueErrorResult);
            }

            ctor.AddCodeStatements(defaultValueResults.Select(x => x.Value!.ToString()));

            var setDefaultValuesMethodNameResult = await _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.SetDefaultValuesMethodName, context.Request.FormatProvider, context.Request, token).ConfigureAwait(false);
            if (!setDefaultValuesMethodNameResult.IsSuccessful())
            {
                return Result.FromExistingResult<ConstructorBuilder>(setDefaultValuesMethodNameResult);
            }

            if (!string.IsNullOrEmpty(setDefaultValuesMethodNameResult.Value!.ToString()))
            {
                ctor.AddCodeStatements($"{setDefaultValuesMethodNameResult.Value}();");
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(setDefaultValuesMethodNameResult.Value)
                    .WithPartial()
                    .WithVisibility(Visibility.Private)
                    );
            }
        }

        return Result.Success(ctor);
    }

    private async Task<List<Result<GenericFormattableString>>> GetDefaultValueResultsAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        var defaultValueResults = new List<Result<GenericFormattableString>>();

        foreach (var property in context.Request.GetSourceProperties()
            .Where
            (x => !x.TypeName.FixTypeName().IsCollectionTypeName()
               && ((!x.IsValueType && !x.IsNullable) || (x.Attributes.Any(y => y.Name == typeof(DefaultValueAttribute).FullName) && context.Request.Settings.UseDefaultValueAttributeValuesForBuilderInitialization))
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

    private async Task<List<Tuple<string, Result<GenericFormattableString>>>> GetConstructorInitializerResultsAsync(PipelineContext<BuilderContext> context)
    {
        var constructorInitializerResults = new List<Tuple<string, Result<GenericFormattableString>>>();

        foreach (var property in context.Request.GetSourceProperties()
            .Where(x => x.TypeName.FixTypeName().IsCollectionTypeName()))
        {
            var name = property.GetBuilderMemberName(context.Request.Settings, context.Request.FormatProvider.ToCultureInfo());
            var result = await property.GetBuilderConstructorInitializerAsync(
                context.Request,
                new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings),
                context.Request.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName),
                context.Request.Settings.BuilderNewCollectionTypeName,
                string.Empty,
                _evaluator).ConfigureAwait(false);

            constructorInitializerResults.Add(new Tuple<string, Result<GenericFormattableString>>(name, result));

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

    private Task<Result<GenericFormattableString>> GenerateDefaultValueStatementAsync(Property property, PipelineContext<BuilderContext> context, CancellationToken token)
        => _evaluator.EvaluateInterpolatedStringAsync
        (
            "{property.BuilderMemberName} = {property.DefaultValue};",
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings),
            token
        );

    private static ConstructorBuilder CreateInheritanceDefaultConstructor(PipelineContext<BuilderContext> context)
        => new ConstructorBuilder()
            .WithChainCall("base()")
            .WithProtected(context.Request.IsBuilderForAbstractEntity);
}
