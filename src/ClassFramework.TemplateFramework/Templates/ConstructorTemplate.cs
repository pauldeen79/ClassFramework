namespace ClassFramework.TemplateFramework.Templates;

public class ConstructorTemplate : CsharpClassGeneratorBase<ConstructorViewModel>, IStringBuilderTemplate
{
    public void Render(StringBuilder builder)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        RenderChildTemplatesByModel(Model.Attributes, builder);

        if (!Model.OmitCode)
        {
            builder.RenderSuppressions(Model.SuppressWarningCodes, "disable", Model.CreateIndentation(1));
        }

        builder.Append(Model.CreateIndentation(1));
        builder.Append(Model.Modifiers);
        builder.Append(Model.Name);
        builder.Append("(");

        RenderChildTemplatesByModel(Model.Parameters, builder);

        builder.Append(")");
        builder.Append(Model.ChainCall);

        if (Model.OmitCode)
        {
            builder.AppendLine(";");
        }
        else
        {
            builder.RenderMethodBody(Model.CreateIndentation(1), () => RenderChildTemplatesByModel(Model.CodeStatements, builder));
            builder.RenderSuppressions(Model.SuppressWarningCodes, "restore", Model.CreateIndentation(1));
        }
    }
}
