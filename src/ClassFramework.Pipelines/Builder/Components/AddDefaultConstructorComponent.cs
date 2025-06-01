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
            var defaultConstructorResult = await CreateDefaultConstructor(context, token).ConfigureAwait(false);
            if (!defaultConstructorResult.IsSuccessful())
            {
                return defaultConstructorResult;
            }

            context.Request.Builder.AddConstructors(defaultConstructorResult.Value!);
        }

        return Result.Success();
    }

    private async Task<Result<ConstructorBuilder>> CreateDefaultConstructor(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        var constructorInitializerResults = (await Task.WhenAll(context.Request.SourceModel.Properties
            .Where(x => context.Request.SourceModel.IsMemberValidForBuilderClass(x, context.Request.Settings) && x.TypeName.FixTypeName().IsCollectionTypeName())
            .Select(async x => new
            {
                Name = x.GetBuilderMemberName(context.Request.Settings, context.Request.FormatProvider.ToCultureInfo()),
                Result = await x.GetBuilderConstructorInitializerAsync(context.Request, new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, x, context.Request.Settings), context.Request.MapTypeName(x.TypeName, MetadataNames.CustomEntityInterfaceTypeName), context.Request.Settings.BuilderNewCollectionTypeName, string.Empty, _evaluator, token).ConfigureAwait(false)
            })).ConfigureAwait(false))
            .TakeWhileWithFirstNonMatching(x => x.Result.IsSuccessful())
            .ToArray();

        var errorResult = Array.Find(constructorInitializerResults, x => !x.Result.IsSuccessful());
        if (errorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(errorResult.Result);
        }

        var ctor = new ConstructorBuilder()
            .WithChainCall(await CreateBuilderClassConstructorChainCall(context.Request.SourceModel, context.Request.Settings).ConfigureAwait(false))
            .WithProtected(context.Request.IsBuilderForAbstractEntity)
            .AddStringCodeStatements(constructorInitializerResults.Select(x => $"{x.Name} = {x.Result.Value};"));

        if (context.Request.Settings.SetDefaultValuesInEntityConstructor)
        {
            var defaultValueResults = (await Task.WhenAll(context.Request.SourceModel.Properties
                .Where
                (x =>
                    context.Request.SourceModel.IsMemberValidForBuilderClass(x, context.Request.Settings)
                    && !x.TypeName.FixTypeName().IsCollectionTypeName()
                    && ((!x.IsValueType && !x.IsNullable) || (x.Attributes.Any(y => y.Name == typeof(DefaultValueAttribute).FullName) && context.Request.Settings.UseDefaultValueAttributeValuesForBuilderInitialization))
                )
                .Select(x => GenerateDefaultValueStatement(x, context, token))).ConfigureAwait(false))
                .TakeWhileWithFirstNonMatching(x => x.IsSuccessful())
                .ToArray();

            var defaultValueErrorResult = Array.Find(defaultValueResults, x => !x.IsSuccessful());
            if (defaultValueErrorResult is not null)
            {
                return Result.FromExistingResult<ConstructorBuilder>(defaultValueErrorResult);
            }

            ctor.AddStringCodeStatements(defaultValueResults.Select(x => x.Value!.ToString()));

            var setDefaultValuesMethodNameResult = await _evaluator.EvaluateInterpolatedStringAsync(context.Request.Settings.SetDefaultValuesMethodName, context.Request.FormatProvider, context.Request, token).ConfigureAwait(false);
            if (!setDefaultValuesMethodNameResult.IsSuccessful())
            {
                return Result.FromExistingResult<ConstructorBuilder>(setDefaultValuesMethodNameResult);
            }

            if (!string.IsNullOrEmpty(setDefaultValuesMethodNameResult.Value!.ToString()))
            {
                ctor.AddStringCodeStatements($"{setDefaultValuesMethodNameResult.Value}();");
                context.Request.Builder.AddMethods(new MethodBuilder()
                    .WithName(setDefaultValuesMethodNameResult.Value)
                    .WithPartial()
                    .WithVisibility(Visibility.Private)
                    );
            }
        }

        return Result.Success(ctor);
    }

    private static async Task<string> CreateBuilderClassConstructorChainCall(IType instance, PipelineSettings settings)
        => (await instance.GetCustomValueForInheritedClassAsync(settings.EnableInheritance, _ => Result.Success<GenericFormattableString>("base()")).ConfigureAwait(false)).Value!; //note that the delegate always returns success, so we can simply use the Value here

    private Task<Result<GenericFormattableString>> GenerateDefaultValueStatement(Property property, PipelineContext<BuilderContext> context, CancellationToken token)
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
