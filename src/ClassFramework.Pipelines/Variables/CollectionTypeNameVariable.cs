namespace ClassFramework.Pipelines.Variables;

public class CollectionTypeNameVariable : IVariable
{
    private readonly IObjectResolver _objectResolver;

    public CollectionTypeNameVariable(IObjectResolver objectResolver)
    {
        ArgumentGuard.IsNotNull(objectResolver, nameof(objectResolver));

        _objectResolver = objectResolver;
    }

    public Result<object?> Process(string variableExpression, object? context)
        => variableExpression switch
        {
            "collectionTypeName" => VariableBase.GetValueFromProperty(_objectResolver, context, (settings, _, _, _) => settings.CollectionTypeName.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics())),
            _ => Result.Continue<object?>()
        };
}
