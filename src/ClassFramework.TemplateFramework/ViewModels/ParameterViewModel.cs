namespace ClassFramework.TemplateFramework.ViewModels;

public class ParameterViewModel(ICsharpExpressionDumper csharpExpressionDumper) : AttributeContainerViewModelBase<Parameter>
{
    public string Prefix
    {
        get
        {
            var model = GetModel();

            if (model.IsParamArray)
            {
                return "params ";
            }
            else if (model.IsRef)
            {
                return "ref ";
            }
            else if (model.IsOut)
            {
                return "out ";
            }

            return string.Empty;
        }
    }

    public string TypeName
        => GetModel().TypeName
            .GetCsharpFriendlyTypeName()
            .AppendNullableAnnotation(Model!.IsNullable, Settings.EnableNullableContext, Model.IsValueType)
            .AbbreviateNamespaces(GetContext().GetCsharpClassGeneratorSettings().IsNotNull(nameof(CsharpClassGeneratorSettings)).NamespacesToAbbreviate);

    public string Name
        => GetModel().Name.Sanitize().GetCsharpFriendlyName();

    public bool ShouldRenderDefaultValue
        => GetModel().DefaultValue is not null;

    public string DefaultValueExpression
        => csharpExpressionDumper.Dump(GetModel().DefaultValue);
}
