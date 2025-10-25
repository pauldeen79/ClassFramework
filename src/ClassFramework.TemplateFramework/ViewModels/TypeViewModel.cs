namespace ClassFramework.TemplateFramework.ViewModels;

public class TypeViewModel : AttributeContainerViewModelBase<IType>
{
    public bool ShouldRenderNullablePragmas
    {
        get
        {
            var settings = Settings;

            return settings.EnableNullableContext
                && settings.EnableNullablePragmas
                && settings.GenerateMultipleFiles // only needed when generating multiple files, because else it will be done only in the header and footer of the generated file
                && Context.GetIndentCount() == 1; // note: only for root level, because it gets rendered in the same file
        }
    }

    public bool ShouldRenderNamespaceScope
        => Settings.GenerateMultipleFiles
        && !string.IsNullOrEmpty(Model.Namespace);

    public string Name
        => Model.Name.Sanitize().GetCsharpFriendlyName();

    public string Namespace
        => Model.Namespace;

    public string Modifiers
        => Model.GetModifiers(Settings.CultureInfo);

    public IReadOnlyCollection<string> SuppressWarningCodes
        => Model.SuppressWarningCodes;

    public CodeGenerationHeaderModel CodeGenerationHeaders
        => new(Settings.CreateCodeGenerationHeader, Settings.EnvironmentVersion);

    public UsingsModel Usings()
        => new([Model]);

    public IEnumerable<object> Members
    {
        get
        {
            var items = new List<object?>();
            var model = Model;

            items.AddRange(model.Fields);
            items.AddRange(model.Properties);

            var constructorsContainer = Model as IConstructorsContainer;
            if (constructorsContainer is not null)
            {
                items.AddRange(constructorsContainer.Constructors);
            }

            items.AddRange(model.Methods);

            if (model is IEnumsContainer enumsContainer)
            {
                items.AddRange(enumsContainer.Enums);
            }

            // Add separators (empty lines) between each item
            return items.SelectMany((item, index) => index + 1 < items.Count ? [item!, new NewLineModel()] : new object[] { item! });
        }
    }

    public IEnumerable<object> SubClasses
    {
        get
        {
            if (Model is not ISubClassesContainer subClassesContainer)
            {
                return [];
            }

            return subClassesContainer.SubClasses.SelectMany(item => new object[] { new NewLineModel(), item });
        }
    }

    public string ContainerType
        => Model switch
        {
            Class cls when cls.Record => "record",
            Class cls when !cls.Record => "class",
            Struct str when str.Record => "record struct",
            Struct str when !str.Record => "struct",
            Interface => "interface",
            _ => throw new NotSupportedException($"Unknown container type: [{Model.GetType().FullName}]")
        };

    public string InheritedClasses
    {
        get
        {
            var baseClassContainer = Model as IBaseClassContainer;

            var lst = new List<string>();

            if (!string.IsNullOrEmpty(baseClassContainer?.BaseClass))
            {
                lst.Add(baseClassContainer.BaseClass);
            }

            lst.AddRange(Model.Interfaces);
            return lst.Count == 0
                ? string.Empty
                : $" : {string.Join(", ", lst.Select(x => x.GetCsharpFriendlyTypeName()))}";
        }
    }

    public string GenericTypeArguments
        => Model.GetGenericTypeArgumentsString();

    public string GenericTypeArgumentConstraints
        => Model.GetGenericTypeArgumentConstraintsString(8 + ((Context.GetIndentCount() - 1) * 4));

    public string FilenamePrefix
        => string.IsNullOrEmpty(Settings.Path)
            ? string.Empty
            : $"{Settings.Path}{Path.DirectorySeparatorChar}";
}
