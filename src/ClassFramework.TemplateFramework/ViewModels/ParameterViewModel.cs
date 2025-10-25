namespace ClassFramework.TemplateFramework.ViewModels;

public class ParameterViewModel(ICsharpExpressionDumper csharpExpressionDumper) : AttributeContainerViewModelBase<Parameter>
{
    public string Prefix
    {
        get
        {
            var model = Model;

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
        => Model.TypeName
            .GetCsharpFriendlyTypeName()
            .AppendNullableAnnotation(Model.IsNullable, Settings.EnableNullableContext, Model.IsValueType)
            .AbbreviateNamespaces(Context.GetCsharpClassGeneratorSettings().IsNotNull(nameof(CsharpClassGeneratorSettings)).NamespacesToAbbreviate);

    public string Name
        => Model.Name.Sanitize().GetCsharpFriendlyName();

    public bool ShouldRenderDefaultValue
        => Model.DefaultValue is not null;

    public string DefaultValueExpression
        => csharpExpressionDumper.Dump(Model.DefaultValue);
}
