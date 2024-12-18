namespace ClassFramework.Pipelines.Variables;

public class ClassVariable : IVariable
{
    private readonly IObjectResolver _objectResolver;

    public ClassVariable(IObjectResolver objectResolver)
    {
        ArgumentGuard.IsNotNull(objectResolver, nameof(objectResolver));

        _objectResolver = objectResolver;
    }
    public Result<object?> Process(string variableExpression, object? context)
        => variableExpression switch
        {
            $"class.{nameof(Class.Name)}" => GetValueFromClass(context, x => x.Name.WithoutGenerics()),
            $"class.{nameof(Class.Namespace)}" => GetValueFromClass(context, x => x.Namespace),
            "class.FullName" => GetValueFromClass(context, x => x.FullName.WithoutGenerics()),
            _ => Result.Continue<object?>()
        };

    private Result<object?> GetValueFromClass(object? context, Func<ClassModel, object?> valueDelegate)
    {
        var classModelResult = _objectResolver.Resolve<ClassModel>(context);
        if (!classModelResult.IsSuccessful())
        {
            return Result.FromExistingResult<object?>(classModelResult);
        }

        return Result.Success(valueDelegate(classModelResult.Value!));
    }
}
