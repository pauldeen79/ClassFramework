namespace ClassFramework.TemplateFramework.ViewModels;

public abstract class CodeStatementViewModelBase<T> : CsharpClassGeneratorViewModelBase<T>
    where T : CodeStatementBase
{
    public int AdditionalIndents
    {
        get
        {
            var parentModel = ParentModel;
            return parentModel switch
            {
                PropertyCodeBodyModel => 3,
                Method or Constructor => 2,
                _ => throw new NotSupportedException($"Don't know how {parentModel?.GetType().FullName ?? "NULL"} should be indented")
            };
        }
    }
}
