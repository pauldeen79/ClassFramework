namespace ClassFramework.TemplateFramework.Templates;

public sealed class UsingsTemplate : CsharpClassGeneratorBase<UsingsViewModel>, IStringBuilderTemplate
{
    public void Render(StringBuilder builder)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        var anyUsings = false;
        foreach (var @using in Model.Usings)
        {
            builder.AppendLine($"using {@using};");
            anyUsings = true;
        }

        if (anyUsings)
        {
            builder.AppendLine();
        }
    }
}
