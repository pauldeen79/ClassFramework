namespace ClassFramework.TemplateFramework.ViewModels;

public abstract class MethodViewModelBase<T> : AttributeContainerViewModelBase<T>
    where T : IAttributesContainer, IParametersContainer, ICodeStatementsContainer, IVisibilityContainer, IMetadataContainer
{
    protected MethodViewModelBase(ICsharpExpressionCreator csharpExpressionCreator)
        : base(csharpExpressionCreator)
    {
    }

    public string Modifiers
        => GetModel().GetModifiers(Settings.CultureInfo);

    public IEnumerable<CodeStatementBase> GetCodeStatementModels()
        => GetModel().CodeStatements;

    public IEnumerable<object> GetParameterModels()
        => GetModel().Parameters
            .SelectMany((item, index) => index + 1 < Model!.Parameters.Count ? [item, new SpaceAndCommaModel()] : new object[] { item });
}
