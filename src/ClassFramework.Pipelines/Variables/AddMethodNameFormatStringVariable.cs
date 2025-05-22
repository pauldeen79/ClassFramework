//namespace ClassFramework.Pipelines.Variables;

//public class AddMethodNameFormatStringVariable : IVariable
//{
//    private readonly IObjectResolver _objectResolver;

//    public AddMethodNameFormatStringVariable(IObjectResolver objectResolver)
//    {
//        ArgumentGuard.IsNotNull(objectResolver, nameof(objectResolver));

//        _objectResolver = objectResolver;
//    }

//    public Result<object?> Evaluate(string expression, object? context)
//        => expression switch
//        {
//            "addMethodNameFormatString" => VariableBase.GetValueFromSettings(_objectResolver, context, settings => settings.AddMethodNameFormatString.WhenNullOrEmpty(() => typeof(List<>).WithoutGenerics())),
//            _ => Result.Continue<object?>()
//        };

//    public Result<Type> Validate(string expression, object? context)
//        => Result.Success(typeof(object));
//}
