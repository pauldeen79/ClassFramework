namespace ClassFramework.TemplateFramework.ViewModels;

public abstract class AttributeContainerViewModelBase<T> : CsharpClassGeneratorViewModelBase<T>
    where T : IAttributesContainer
{
    protected AttributeContainerViewModelBase(ICsharpExpressionDumper csharpExpressionDumper)
        : base(csharpExpressionDumper)
    {
    }

    public IEnumerable<Domain.Attribute> Attributes => GetModel().Attributes;
}
