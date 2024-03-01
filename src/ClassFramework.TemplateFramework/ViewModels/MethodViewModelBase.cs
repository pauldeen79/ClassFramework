namespace ClassFramework.TemplateFramework.ViewModels;

public abstract class MethodViewModelBase<T> : AttributeContainerViewModelBase<T>
    where T : IAttributesContainer, IParametersContainer, ICodeStatementsContainer, IVisibilityContainer, IMetadataContainer, ISuppressWarningCodesContainer
{
    protected MethodViewModelBase(ICsharpExpressionCreator csharpExpressionCreator)
        : base(csharpExpressionCreator)
    {
    }

    public string Modifiers
        => GetModel().GetModifiers(Settings.CultureInfo);

    public IReadOnlyCollection<string> SuppressWarningCodes
        => GetModel().SuppressWarningCodes;

    public IEnumerable<CodeStatementBase> CodeStatements
        => GetModel().CodeStatements;

    public IEnumerable<object> Parameters
        => GetModel().Parameters
            .SelectMany((item, index) => index + 1 < Model!.Parameters.Count ? [item, new SpaceAndCommaModel()] : new object[] { item });
}
