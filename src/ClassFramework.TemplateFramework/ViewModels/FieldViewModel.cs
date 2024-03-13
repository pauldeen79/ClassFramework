namespace ClassFramework.TemplateFramework.ViewModels;

public class FieldViewModel : AttributeContainerViewModelBase<Field>
{
    public FieldViewModel(ICsharpExpressionDumper csharpExpressionDumper)
        : base(csharpExpressionDumper)
    {
    }

    public string Modifiers
        => GetModel().GetModifiers(Settings.CultureInfo);

    public bool Event
        => GetModel().Event;

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
