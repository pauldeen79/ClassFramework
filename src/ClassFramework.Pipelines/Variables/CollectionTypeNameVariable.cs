namespace ClassFramework.Pipelines.Variables;

public class CollectionTypeNameVariable : IVariable
{
    private readonly IObjectResolver _objectResolver;

    public CollectionTypeNameVariable(IObjectResolver objectResolver)
    {
        ArgumentGuard.IsNotNull(objectResolver, nameof(objectResolver));

        _objectResolver = objectResolver;
    }

    public Result<object?> Evaluate(string expression, object? context)
        => expression switch
        {
            "collectionTypeName" => VariableBase.GetValueFromSettings(_objectResolver, context, settings => settings.CollectionTypeName.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics())),
            _ => Result.Continue<object?>()
        };

    public Result Validate(string expression, object? context)
        => Result.Success();
}
