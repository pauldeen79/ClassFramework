namespace ClassFramework.Pipelines.Shared.Variables;

public class NullableRequiredSuffixVariable : IVariable
{
    public Result<object?> Process(string variableExpression, object? context)
    {
        // seems this is only necessary on entity context?
        if (variableExpression == "nullableRequiredSuffix" && context is ParentChildContext<PipelineContext<EntityContext>, Property> parentChildContextOfEntity)
        {
            var value = !parentChildContextOfEntity.ParentContext.Request.Settings.AddNullChecks && !parentChildContextOfEntity.ChildContext.IsValueType && !parentChildContextOfEntity.ChildContext.IsNullable && parentChildContextOfEntity.ParentContext.Request.Settings.EnableNullableReferenceTypes
                ? "!"
                : string.Empty;

            return Result.Success<object?>(value);
        }

        return Result.Continue<object?>();
    }
}
