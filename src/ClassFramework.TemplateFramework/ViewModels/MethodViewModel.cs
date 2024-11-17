namespace ClassFramework.TemplateFramework.ViewModels;

public class MethodViewModel : MethodViewModelBase<Method>
{
    public bool ShouldRenderModifiers
        => string.IsNullOrEmpty(GetModel().ExplicitInterfaceName) && GetParentModel() is not Interface;

    public string ReturnTypeName
        => GetModel().ReturnTypeName
            .GetCsharpFriendlyTypeName()
            .AppendNullableAnnotation(Model!.ReturnTypeIsNullable, Settings.EnableNullableContext, Model.ReturnTypeIsValueType)
            .AbbreviateNamespaces(GetContext().GetCsharpClassGeneratorSettings().IsNotNull(nameof(CsharpClassGeneratorSettings)).NamespacesToAbbreviate)
            .WhenNullOrEmpty("void");

    public string ExplicitInterfaceName
        => !string.IsNullOrEmpty(GetModel().ExplicitInterfaceName) && GetParentModel() is not Interface
            ? $"{Model!.ExplicitInterfaceName}."
            : string.Empty;

    public string GenericTypeArguments
        => GetModel().GetGenericTypeArgumentsString();

    public string GenericTypeArgumentConstraints
        => GetModel().GetGenericTypeArgumentConstraintsString(12 + ((GetContext().GetIndentCount() - 1) * 4));

    public bool ExtensionMethod
        => GetModel().ExtensionMethod;

    public string Name
    {
        get
        {
            var model = GetModel();
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
            var model = GetModel();

            return GetParentModel() is Interface || model.Abstract || model.Partial;
        }
    }
}
