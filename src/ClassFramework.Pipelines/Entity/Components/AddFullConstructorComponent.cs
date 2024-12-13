namespace ClassFramework.Pipelines.Entity.Components;

public class AddFullConstructorComponentBuilder(IFormattableStringParser formattableStringParser) : IEntityComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public IPipelineComponent<EntityContext> Build()
        => new AddFullConstructorComponent(_formattableStringParser);
}

public class AddFullConstructorComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<EntityContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> Process(PipelineContext<EntityContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AddFullConstructor)
        {
            return Task.FromResult(Result.Continue());
        }

        var ctorResult = CreateEntityConstructor(context);
        if (!ctorResult.IsSuccessful())
        {
            return Task.FromResult<Result>(ctorResult);
        }

        context.Request.Builder.AddConstructors(ctorResult.Value!);

        if (context.Request.Settings.AddValidationCode() == ArgumentValidationType.CustomValidationCode)
        {
            context.Request.Builder.AddMethods(new MethodBuilder().WithName("Validate").WithPartial().WithVisibility(Visibility.Private));
        }

        return Task.FromResult(Result.Continue());
    }

    private Result<ConstructorBuilder> CreateEntityConstructor(PipelineContext<EntityContext> context)
    {
        var initializationResults = context.Request.SourceModel.Properties
            .Where(property => context.Request.SourceModel.IsMemberValidForBuilderClass(property, context.Request.Settings))
            .Select(property => _formattableStringParser.Parse("this.{$property.EntityMemberName} = {$property.InitializationExpression};", context.Request.FormatProvider, new ParentChildContext<PipelineContext<EntityContext>, Property>(context, property, context.Request.Settings)))
            .TakeWhileWithFirstNonMatching(x => x.IsSuccessful())
            .ToArray();

        var error = Array.Find(initializationResults, x => !x.IsSuccessful());
        if (error is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(error);
        }

        return Result.Success(new ConstructorBuilder()
            .WithProtected(context.Request.Settings.EnableInheritance && context.Request.Settings.IsAbstract)
            .AddParameters(context.Request.SourceModel.Properties.CreateImmutableClassCtorParameters(context.Request.FormatProvider, n => context.Request.MapTypeName(n, MetadataNames.CustomEntityInterfaceTypeName)))
            .AddStringCodeStatements
            (
                context.Request.SourceModel.Properties
                    .Where(property => context.Request.SourceModel.IsMemberValidForBuilderClass(property, context.Request.Settings))
                    .Where(property => context.Request.Settings.AddNullChecks && context.Request.Settings.AddValidationCode() == ArgumentValidationType.None && context.Request.GetMappingMetadata(property.TypeName).GetValue(MetadataNames.EntityNullCheck, () => !property.IsNullable && !property.IsValueType))
                    .Select(property => context.Request.CreateArgumentNullException(property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo()).GetCsharpFriendlyName()))
            )
            .AddStringCodeStatements(initializationResults.Select(x => x.Value!.ToString()))
            .AddStringCodeStatements(context.Request.CreateEntityValidationCode())
            .WithChainCall(context.CreateEntityChainCall()));
    }
}
