namespace ClassFramework.TemplateFramework.ViewModels;

public class ConstructorViewModel : MethodViewModelBase<Constructor>
{
    public string Name
    {
        get
        {
            var parentModel = GetParentModel();

            if (parentModel is not IType nameContainer)
            {
                throw new NotSupportedException($"Type {parentModel?.GetType().FullName ?? "NULL"} is not supported for constructors. Only class implementing IType are supported.");
            }

            return nameContainer.Name.Sanitize().GetCsharpFriendlyName();
        }
    }

    public string ChainCall
        => string.IsNullOrEmpty(GetModel().ChainCall)
            ? string.Empty
            : $" : {Model!.ChainCall}";

    public bool OmitCode
        => GetParentModel() is Interface || GetModel().Abstract;
}
