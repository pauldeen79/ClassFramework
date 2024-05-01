namespace ClassFramework.TemplateFramework.ViewModels;

public class PropertyViewModel : AttributeContainerViewModelBase<Property>
{
    public PropertyViewModel(ICsharpExpressionDumper csharpExpressionDumper)
    {
        CsharpExpressionDumper = csharpExpressionDumper;
    }

    private ICsharpExpressionDumper CsharpExpressionDumper { get; }

    public bool ShouldRenderModifiers
        => GetParentModel() is not Interface;
    
    public string TypeName
        => GetModel().TypeName
            .GetCsharpFriendlyTypeName()
            .AppendNullableAnnotation(Model!.IsNullable, Settings.EnableNullableContext)
            .AbbreviateNamespaces(GetContext().GetCsharpClassGeneratorSettings().IsNotNull(nameof(CsharpClassGeneratorSettings)).NamespacesToAbbreviate);

    public string Name
        => GetModel().Name.Sanitize().GetCsharpFriendlyName();

    public string Modifiers
        => GetModel().GetModifiers(Settings.CultureInfo);

    public string ExplicitInterfaceName
        => !string.IsNullOrEmpty(GetModel().ExplicitInterfaceName) && GetParentModel() is not Interface
            ? $"{Model!.ExplicitInterfaceName}."
            : string.Empty;

    public bool ShouldRenderDefaultValue
        => GetModel().DefaultValue is not null;

    public string DefaultValueExpression
        => CsharpExpressionDumper.Dump(GetModel().DefaultValue);

    public IEnumerable<PropertyCodeBodyModel> CodeBodyItems
    {
        get
        {
            var model = GetModel();
            var parentModel = GetParentModel();

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
