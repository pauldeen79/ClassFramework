namespace ClassFramework.TemplateFramework.ViewModels;

public class FieldViewModel(ICsharpExpressionDumper csharpExpressionDumper) : AttributeContainerViewModelBase<Field>
{
    public string Modifiers
        => Model.GetModifiers(Settings.CultureInfo);

    public bool Event
        => Model.Event;

    public string TypeName
        => Model.TypeName
            .GetCsharpFriendlyTypeName()
            .AppendNullableAnnotation(Model!.IsNullable, Settings.EnableNullableContext, Model.IsValueType)
            .AbbreviateNamespaces(Context.GetCsharpClassGeneratorSettings().IsNotNull(nameof(CsharpClassGeneratorSettings)).NamespacesToAbbreviate);

    public string Name
        => Model.Name.Sanitize().GetCsharpFriendlyName();

    public bool ShouldRenderDefaultValue
        => Model.DefaultValue is not null;

    public string DefaultValueExpression
        => csharpExpressionDumper.Dump(Model.DefaultValue);
}
