namespace ClassFramework.TemplateFramework.ViewModels;

public abstract class MethodViewModelBase<T> : AttributeContainerViewModelBase<T>
    where T : IAttributesContainer, IParametersContainer, ICodeStatementsContainer, IVisibilityContainer, ISuppressWarningCodesContainer
{
    public string Modifiers
    {
        get
        {
            if (GetParentModel() is Interface && GetModel() is IModifiersContainer modifiersContainer && modifiersContainer.New)
            {
                return "new ";
            }

            return GetModel().GetModifiers(Settings.CultureInfo);
        }
    }

    public IReadOnlyCollection<string> SuppressWarningCodes
        => GetModel().SuppressWarningCodes;

    public IEnumerable<CodeStatementBase> CodeStatements
        => GetModel().CodeStatements;

    public IEnumerable<object> Parameters
        => GetModel().Parameters
            .SelectMany((item, index) => index + 1 < Model!.Parameters.Count ? [item, new SpaceAndCommaModel()] : new object[] { item });
}
