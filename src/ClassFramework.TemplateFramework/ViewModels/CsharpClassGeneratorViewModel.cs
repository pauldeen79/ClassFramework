namespace ClassFramework.TemplateFramework.ViewModels;

public class CsharpClassGeneratorViewModel : CsharpClassGeneratorViewModelBase<IEnumerable<TypeBase>>
{
    public CsharpClassGeneratorViewModel(ICsharpExpressionDumper csharpExpressionDumper)
        : base(csharpExpressionDumper)
    {
    }

    public IOrderedEnumerable<IGrouping<string, TypeBase>> Namespaces
        => GetModel().GroupBy(x => x.Namespace).OrderBy(x => x.Key);

    public CodeGenerationHeaderModel GetCodeGenerationHeaderModel()
        => new CodeGenerationHeaderModel(Settings.CreateCodeGenerationHeader, Settings.EnvironmentVersion);

    public UsingsModel Usings
        => new UsingsModel(GetModel());

    public IEnumerable<TypeBase> GetTypes(IEnumerable<TypeBase> @namespace)
        => @namespace.OrderBy(typeBase => typeBase.Name);

    public bool ShouldRenderNullablePragmas
    {
        get
        {
            var settings = GetSettings();

            return settings.EnableNullableContext
                && !settings.GenerateMultipleFiles; // only needed when generating single file
        }
    }
}
