namespace ClassFramework.TemplateFramework.ViewModels;

public class ConstructorViewModel : MethodViewModelBase<Constructor>
{
    public ConstructorViewModel(ICsharpExpressionCreator csharpExpressionCreator)
        : base(csharpExpressionCreator)
    {
    }

    public string Name
    {
        get
        {   
            var parentModel = GetParentModel();

            var nameContainer = parentModel as IType;
            if (nameContainer is null)
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
