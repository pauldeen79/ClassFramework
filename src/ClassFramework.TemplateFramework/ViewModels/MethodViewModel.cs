namespace ClassFramework.TemplateFramework.ViewModels;

public class MethodViewModel : MethodViewModelBase<Method>
{
    public bool ShouldRenderModifiers
        => (string.IsNullOrEmpty(Model.ExplicitInterfaceName) && ParentModel is not Interface)
        || (ParentModel is Interface && Model.New);

    public string ReturnTypeName
        => Model.ReturnTypeName
            .GetCsharpFriendlyTypeName()
            .AppendNullableAnnotation(Model!.ReturnTypeIsNullable, Settings.EnableNullableContext, Model.ReturnTypeIsValueType)
            .AbbreviateNamespaces(Context.GetCsharpClassGeneratorSettings().IsNotNull(nameof(CsharpClassGeneratorSettings)).NamespacesToAbbreviate)
            .WhenNullOrEmpty("void");

    public string ReturnTypeGenericTypeArguments
        => Model.ReturnTypeGenericTypeArguments.Select(x => x.TypeName).GetGenericTypeArgumentsString();

    public string ExplicitInterfaceName
        => !string.IsNullOrEmpty(Model.ExplicitInterfaceName) && ParentModel is not Interface
            ? $"{Model!.ExplicitInterfaceName}."
            : string.Empty;

    public string GenericTypeArguments
        => Model.GetGenericTypeArgumentsString();

    public string GenericTypeArgumentConstraints
        => Model.GetGenericTypeArgumentConstraintsString(12 + ((Context.GetIndentCount() - 1) * 4));

    public bool ExtensionMethod
        => Model.ExtensionMethod;

    public string Name
    {
        get
        {
            var model = Model;
            if (model.Operator)
            {
                return "operator " + model.Name;
            }

            if (model.IsInterfaceMethod())
            {
                return model.Name.RemoveInterfacePrefix().Sanitize();
            }

            return model.Name.Sanitize().GetCsharpFriendlyName();
        }
    }

    public bool OmitCode
    {
        get
        {
            var model = Model;

            return ParentModel is Interface || model.Abstract || model.Partial;
        }
    }
}
