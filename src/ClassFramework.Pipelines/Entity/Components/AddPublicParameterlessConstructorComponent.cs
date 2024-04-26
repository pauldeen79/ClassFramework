namespace ClassFramework.Pipelines.Entity.Features;

public class AddPublicParameterlessConstructorComponentBuilder : IEntityComponentBuilder
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddPublicParameterlessConstructorComponentBuilder(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public IPipelineComponent<EntityContext, IConcreteTypeBuilder> Build()
        => new AddPublicParameterlessConstructorComponent(_formattableStringParser);
}

public class AddPublicParameterlessConstructorComponent : IPipelineComponent<EntityContext, IConcreteTypeBuilder>
{
    private readonly IFormattableStringParser _formattableStringParser;

    public AddPublicParameterlessConstructorComponent(IFormattableStringParser formattableStringParser)
    {
        _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));
    }

    public Task<Result> Process(PipelineContext<EntityContext, IConcreteTypeBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.AddPublicParameterlessConstructor)
        {
            return Task.FromResult(Result.Continue());
        }

        var ctorResult = CreateEntityConstructor(context);
        if (!ctorResult.IsSuccessful())
        {
            return Task.FromResult<Result>(ctorResult);
        }

        context.Response.AddConstructors(ctorResult.Value!);

        return Task.FromResult(Result.Continue());
    }

    private Result<ConstructorBuilder> CreateEntityConstructor(PipelineContext<EntityContext, IConcreteTypeBuilder> context)
    {
        var initializationStatements = context.Request.SourceModel.Properties
            .Where(x => context.Request.SourceModel.IsMemberValidForBuilderClass(x, context.Request.Settings))
            .Select(x => GenerateDefaultValueStatement(x, context))
            .ToArray();

        var errorResult = Array.Find(initializationStatements, x => !x.IsSuccessful());
        if (errorResult is not null)
        {
            return Result.FromExistingResult<ConstructorBuilder>(errorResult);
        }

        return Result.Success(new ConstructorBuilder()
            .AddStringCodeStatements(initializationStatements.Select(x => x.Value!))
            );
    }

    private Result<string> GenerateDefaultValueStatement(Property property, PipelineContext<EntityContext, IConcreteTypeBuilder> context)
        => _formattableStringParser.Parse
        (
            property.TypeName.FixTypeName().IsCollectionTypeName()
                ? "{EntityMemberName} = new {CollectionTypeName}<{TypeName.GenericArguments}>();"
                : "{EntityMemberName} = {DefaultValue};",
            context.Request.FormatProvider,
            new ParentChildContext<PipelineContext<EntityContext, IConcreteTypeBuilder>, Property>(context, property, context.Request.Settings)
        ).TransformValue(x => x.ToString());
}
