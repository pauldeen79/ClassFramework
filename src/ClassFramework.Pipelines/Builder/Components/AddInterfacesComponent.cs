
namespace ClassFramework.Pipelines.Builder.Components;

public class AddInterfacesComponent(IFormattableStringParser formattableStringParser) : IPipelineComponent<BuilderContext>
{
    private readonly IFormattableStringParser _formattableStringParser = formattableStringParser.IsNotNull(nameof(formattableStringParser));

    public Task<Result> ProcessAsync(PipelineContext<BuilderContext> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyInterfaces)
        {
            return Task.FromResult(Result.Success());
        }

        var results = context.Request.SourceModel.Interfaces
            .Where(x => context.Request.Settings.CopyInterfacePredicate?.Invoke(x) ?? true)
            .Select(x =>
            {
                var metadata = context.Request.GetMappingMetadata(x);
                var ns = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceNamespace);

                if (!string.IsNullOrEmpty(ns))
                {
                    var property = new PropertyBuilder()
                        .WithName("Dummy")
                        .WithTypeName(x.WithoutGenerics())
                        .AddGenericTypeArguments(x.GetGenericArguments().Split(',').Select(y => new TypeContainer(y)))
                        .Build();
                    var newTypeName = metadata.GetStringValue(MetadataNames.CustomBuilderInterfaceName, "{NoGenerics(ClassName($property.TypeName))}Builder{GenericArguments($property.TypeName, true)}");
                    var newFullName = $"{ns}.{newTypeName}";

                    return _formattableStringParser.Parse
                    (
                        newFullName,
                        context.Request.FormatProvider,
                        new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings)
                    ).Transform(x => x.ToString());
                }
                return Result.Success(context.Request.MapTypeName(x.FixTypeName()));
            })
            .TakeWhileWithFirstNonMatching(x => x.IsSuccessful())
            .ToArray();

        var error = Array.Find(results, x => !x.IsSuccessful());
        if (error is not null)
        {
            return Task.FromResult<Result>(error);
        }

        context.Request.Builder.AddInterfaces(results.Select(x => x.Value!));

        return Task.FromResult(Result.Success());
    }

    private sealed class TypeContainer : ITypeContainer
    {
        public string TypeName { get; }

        public bool IsNullable => false;

        public bool IsValueType => false;

        public IReadOnlyCollection<ITypeContainer> GenericTypeArguments => new List<ITypeContainer>();

        public TypeContainer(string typeName)
        {
            TypeName = typeName;
        }
    }
}
