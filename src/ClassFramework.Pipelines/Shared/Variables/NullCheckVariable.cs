namespace ClassFramework.Pipelines.Shared.Variables;

public class NullCheckVariable : IVariable
{
    public Result<object?> Process(string variableExpression, object? context)
    {
        if (variableExpression == "nullCheck")
        {
            var value = context switch
            {
                ContextBase contextBase => contextBase.NullCheck,
                ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextEntity => parentChildContextEntity.ParentContext.Request.NullCheck,
                _ => string.Empty
            };

            
            return Result.Success<object?>(value);
        }

        return Result.Continue<object?>();
    }
}
