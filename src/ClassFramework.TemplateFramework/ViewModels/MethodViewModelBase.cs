namespace ClassFramework.TemplateFramework.ViewModels;

public abstract class MethodViewModelBase<T> : AttributeContainerViewModelBase<T>
    where T : IAttributesContainer, IParametersContainer, ICodeStatementsContainer, IVisibilityContainer, ISuppressWarningCodesContainer
{
    public string Modifiers
    {
        get
        {
            if (ParentModel is Interface && Model is IModifiersContainer modifiersContainer && modifiersContainer.New)
            {
                return "new ";
            }

            return Model.GetModifiers(Settings.CultureInfo);
        }
    }

    public IReadOnlyCollection<string> SuppressWarningCodes
        => Model.SuppressWarningCodes;

    public IEnumerable<CodeStatementBase> CodeStatements
        => Model.CodeStatements;

    public IEnumerable<object> Parameters
        => Model.Parameters
            .SelectMany((item, index) => index + 1 < Model.Parameters.Count ? [item, new SpaceAndCommaModel()] : new object[] { item });
}
