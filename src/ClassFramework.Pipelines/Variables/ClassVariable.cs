namespace ClassFramework.Pipelines.Variables;

public class ClassVariable : IVariable
{
    private readonly IObjectResolver _objectResolver;

    public ClassVariable(IObjectResolver objectResolver)
    {
        ArgumentGuard.IsNotNull(objectResolver, nameof(objectResolver));

        _objectResolver = objectResolver;
    }

    public Result<object?> Evaluate(string expression, object? context)
        => expression switch
        {
            $"class.{nameof(Class.Name)}" => GetValueFromClass(context, x => x.Name.WithoutTypeGenerics()),
            $"class.{nameof(Class.Namespace)}" => GetValueFromClass(context, x => x.Namespace),
            "class.FullName" => GetValueFromClass(context, x => x.FullName.WithoutTypeGenerics()),
            "class.GenericTypeArguments" => GetValueFromClass(context, x => x.GenericArguments),
            _ => Result.Continue<object?>()
        };

    public Result<Type> Validate(string expression, object? context)
        => Result.Success(typeof(object));

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
