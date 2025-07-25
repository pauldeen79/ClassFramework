namespace ClassFramework.TemplateFramework.ViewModels;

public class PropertyViewModel(ICsharpExpressionDumper csharpExpressionDumper) : AttributeContainerViewModelBase<Property>
{
    public bool ShouldRenderModifiers
        => ParentModel is not Interface;

    public string TypeName
        => Model.TypeName
            .GetCsharpFriendlyTypeName()
            .AppendNullableAnnotation(Model!.IsNullable, Settings.EnableNullableContext, Model.IsValueType)
            .AbbreviateNamespaces(Context.GetCsharpClassGeneratorSettings().IsNotNull(nameof(CsharpClassGeneratorSettings)).NamespacesToAbbreviate);

    public string Name
        => Model.Name.Sanitize().GetCsharpFriendlyName();

    public string Modifiers
        => Model.GetModifiers(Settings.CultureInfo);

    public string ExplicitInterfaceName
        => !string.IsNullOrEmpty(Model.ExplicitInterfaceName) && ParentModel is not Interface
            ? $"{Model!.ExplicitInterfaceName}."
            : string.Empty;

    public bool ShouldRenderDefaultValue
        => Model.DefaultValue is not null;

    public string DefaultValueExpression
        => csharpExpressionDumper.Dump(Model.DefaultValue);

    public IEnumerable<PropertyCodeBodyModel> CodeBodyItems
    {
        get
        {
            var model = Model;
            var parentModel = ParentModel;

            if (model.HasGetter)
            {
                yield return new PropertyCodeBodyModel("get", model.Visibility, model.GetterVisibility, parentModel, model.GetterCodeStatements, Settings.CultureInfo);
            }

            if (model.HasInitializer)
            {
                yield return new PropertyCodeBodyModel("init", model.Visibility, model.InitializerVisibility, parentModel, model.InitializerCodeStatements, Settings.CultureInfo);
            }

            if (model.HasSetter)
            {
                yield return new PropertyCodeBodyModel("set", model.Visibility, model.SetterVisibility, parentModel, model.SetterCodeStatements, Settings.CultureInfo);
            }
        }
    }
}
