namespace ClassFramework.TemplateFramework.ViewModels;

public class FieldViewModel : AttributeContainerViewModelBase<Field>
{
    public string Modifiers
        => GetModel().GetModifiers(Settings.CultureInfo);

    public bool Event
        => GetModel().Event;

    public string TypeName
        => GetModel().TypeName
            .GetCsharpFriendlyTypeName()
            .AppendNullableAnnotation(Model!.IsNullable, Settings.EnableNullableContext)
            .AbbreviateNamespaces(GetContext().GetCsharpClassGeneratorSettings().IsNotNull(nameof(CsharpClassGeneratorSettings)).NamespacesToAbbreviate);

    public string Name
        => GetModel().Name.Sanitize().GetCsharpFriendlyName();

    public bool ShouldRenderDefaultValue
        => GetModel().DefaultValue is not null;

    public string DefaultValueExpression
        => GetCsharpExpression(GetModel().DefaultValue);
}
