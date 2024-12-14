namespace ClassFramework.Pipelines.Variables;

public class CollectionTypeNameVariable : IVariable
{
    public Result<object?> Process(string variableExpression, object? context)
        => variableExpression switch
        {
            "collectionTypeName" => VariableBase.GetValueFromProperty(context, (settings, _, _, _) => settings.CollectionTypeName.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics())),
            _ => Result.Continue<object?>()
        };
}
