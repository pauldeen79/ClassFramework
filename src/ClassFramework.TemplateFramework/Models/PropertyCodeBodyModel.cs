namespace ClassFramework.TemplateFramework.Models;

public class PropertyCodeBodyModel
{
    public PropertyCodeBodyModel(string verb, Visibility visibilty, SubVisibility subVisibility, object? parentModel, IReadOnlyCollection<CodeStatementBase> codeStatementModels, CultureInfo cultureInfo)
    {
        Guard.IsNotNull(verb);
        Guard.IsNotNull(codeStatementModels);
        Guard.IsNotNull(cultureInfo);

        Verb = verb;
        Modifiers = GetModifiers(visibilty, subVisibility, cultureInfo);
        OmitCode = codeStatementModels.Count == 0 || parentModel is TypeViewModel vm && vm.Model is Interface;
        CodeStatementModels = codeStatementModels;
    }

    public string Verb { get; }
    public string Modifiers { get; }
    public bool OmitCode { get; }
    public IReadOnlyCollection<CodeStatementBase> CodeStatementModels { get; }

    private static string GetModifiers(Visibility visibility, SubVisibility subVisibility, CultureInfo cultureInfo)
    {
        var builder = new StringBuilder();

        if (subVisibility != SubVisibility.InheritFromParent && subVisibility.ToString() != visibility.ToString())
        {
            builder.Append(subVisibility.ToString().ToLower(cultureInfo));
        }

        if (builder.Length > 0)
        {
            builder.Append(" ");
        }

        return builder.ToString();
    }
}
