﻿namespace ClassFramework.TemplateFramework.ViewModels;

public class ParameterViewModel : AttributeContainerViewModelBase<Parameter>
{
    public ParameterViewModel(ICsharpExpressionDumper csharpExpressionDumper)
        : base(csharpExpressionDumper)
    {
    }

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
            .AppendNullableAnnotation(Model!.IsNullable, Settings.EnableNullableContext)
            .AbbreviateNamespaces(Model.Metadata.GetStringValues(MetadataNames.NamespaceToAbbreviate));

    public string Name
        => GetModel().Name.Sanitize().GetCsharpFriendlyName();

    public bool ShouldRenderDefaultValue
        => GetModel().DefaultValue is not null;

    public string DefaultValueExpression
        => CsharpExpressionDumper.Dump(GetModel().DefaultValue);
}
