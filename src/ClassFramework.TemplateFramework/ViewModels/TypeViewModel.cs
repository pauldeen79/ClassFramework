namespace ClassFramework.TemplateFramework.ViewModels;

public class TypeViewModel : AttributeContainerViewModelBase<IType>
{
    public TypeViewModel(ICsharpExpressionDumper csharpExpressionDumper)
        : base(csharpExpressionDumper)
    {
    }

    public bool ShouldRenderNullablePragmas
        => GetSettings().EnableNullableContext
        && GetContext().GetIndentCount() == 1; // note: only for root level, because it gets rendered in the same file

    public bool ShouldRenderNamespaceScope
        => GetSettings().GenerateMultipleFiles
        && !string.IsNullOrEmpty(GetModel().Namespace);

    public string Name
        => GetModel().Name.Sanitize().GetCsharpFriendlyName();

    public string Namespace
        => GetModel().Namespace;

    public string Modifiers
        => GetModel().GetModifiers(GetSettings().CultureInfo);

    public IReadOnlyCollection<string> SuppressWarningCodes
        => GetModel().SuppressWarningCodes;

    public CodeGenerationHeaderModel CodeGenerationHeaders
        => new CodeGenerationHeaderModel(Settings.CreateCodeGenerationHeader, Settings.EnvironmentVersion);

    public UsingsModel Usings()
        => new UsingsModel([GetModel()]);

    public IEnumerable<object> Members
    {
        get
        {
            var items = new List<object?>();
            var model = GetModel();

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
            if (GetModel() is not ISubClassesContainer subClassesContainer)
            {
                return Enumerable.Empty<object>();
            }

            return subClassesContainer.SubClasses.SelectMany(item => new object[] { new NewLineModel(), item });
        }
    }

    public string ContainerType
        => GetModel() switch
        {
            Class cls when cls.Record => "record",
            Class cls when !cls.Record => "class",
            Struct str when str.Record => "record struct",
            Struct str when !str.Record => "struct",
            Interface => "interface",
            _ => throw new NotSupportedException($"Unknown container type: [{Model!.GetType().FullName}]")
        };

    public string InheritedClasses
    {
        get
        {
            var baseClassContainer = GetModel() as IBaseClassContainer;

            var lst = new List<string>();

            if (!string.IsNullOrEmpty(baseClassContainer?.BaseClass))
            {
                lst.Add(baseClassContainer.BaseClass);
            }

            lst.AddRange(Model!.Interfaces);
            return lst.Count == 0
                ? string.Empty
                : $" : {string.Join(", ", lst.Select(x => x.GetCsharpFriendlyTypeName()))}";
        }
    }

    public string GenericTypeArguments
        => GetModel().GetGenericTypeArgumentsString();

    public string GenericTypeArgumentConstraints
        => GetModel().GetGenericTypeArgumentConstraintsString(8 + ((GetContext().GetIndentCount() - 1) * 4));

    public string FilenamePrefix
        => string.IsNullOrEmpty(GetSettings().Path)
            ? string.Empty
            : $"{Settings.Path}{Path.DirectorySeparatorChar}";
}
