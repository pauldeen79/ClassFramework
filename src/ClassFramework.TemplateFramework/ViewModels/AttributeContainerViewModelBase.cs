namespace ClassFramework.TemplateFramework.ViewModels;

public abstract class AttributeContainerViewModelBase<T> : CsharpClassGeneratorViewModelBase<T>
    where T : IAttributesContainer
{
    public IEnumerable<Domain.Attribute> Attributes => Model.Attributes;
}
