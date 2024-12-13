namespace ClassFramework.Pipelines.Shared.Variables;

public class NullCheckVariable : IVariable
{
    public Result<object?> Process(string variableExpression, object? context)
    {
        if (variableExpression == "nullCheck")
        {
            return context switch
            {
                ContextBase contextBase => Result.Success<object?>(contextBase.NullCheck),
                ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => Result.Success<object?>(parentChildContextEntity.ParentContext.Request.NullCheck),
                _ => Result.Invalid<object?>($"Could not get null check from context, because the context type {context?.GetType().FullName ?? "null"} is not supported")
            };
        }

        return Result.Continue<object?>();
    }
}
