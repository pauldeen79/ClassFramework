namespace ClassFramework.TemplateFramework.ViewModels;

public class AttributeViewModel : CsharpClassGeneratorViewModelBase<Domain.Attribute>
{
    public bool IsSingleLineAttributeContainer => GetParentModel() is Parameter;

    public string Name
        => GetModel().Name.GetCsharpFriendlyName(); // do not sanitize, as the name may contain dots (.) for namesapce separators

    public string Parameters
        => GetModel().Parameters.Count == 0
            ? string.Empty
            : string.Concat("(", string.Join(", ", Model!.Parameters.Select(p =>
                string.IsNullOrEmpty(p.Name)
                    ? GetCsharpExpression(p.Value)
                    : $"{p.Name} = {GetCsharpExpression(p.Value)}"
            )), ")");

    public int AdditionalIndents
    {
        get
        {
            if (IsSingleLineAttributeContainer || GetParentModel() is IType)
            {
                return 0;
            }

            return 1;
        }
    }
}
