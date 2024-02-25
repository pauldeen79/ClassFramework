namespace ClassFramework.TemplateFramework.Models;

public class UsingsModel
{
    public IEnumerable<IType> Types { get; }

    public UsingsModel(IEnumerable<IType> types)
    {
        Guard.IsNotNull(types);

        Types = types;
    }
}
