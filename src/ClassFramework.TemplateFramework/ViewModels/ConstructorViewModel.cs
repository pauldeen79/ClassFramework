namespace ClassFramework.TemplateFramework.ViewModels;

public class ConstructorViewModel : MethodViewModelBase<Constructor>
{
    public string Name
    {
        get
        {
            var parentModel = ParentModel;

            if (parentModel is not IType nameContainer)
            {
                throw new NotSupportedException($"Type {parentModel?.GetType().FullName ?? "NULL"} is not supported for constructors. Only class implementing IType are supported.");
            }

            return nameContainer.Name.Sanitize().GetCsharpFriendlyName();
        }
    }

    public string ChainCall
        => string.IsNullOrEmpty(Model.ChainCall)
            ? string.Empty
            : $" : {Model.ChainCall}";

    public bool OmitCode
        => ParentModel is Interface || Model.Abstract;
}
