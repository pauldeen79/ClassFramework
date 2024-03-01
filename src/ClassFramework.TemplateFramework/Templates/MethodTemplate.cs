namespace ClassFramework.TemplateFramework.Templates;

public class MethodTemplate : CsharpClassGeneratorBase<MethodViewModel>, IStringBuilderTemplate
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

        if (Model.ShouldRenderModifiers)
        {
            builder.Append(Model.Modifiers);
        }

        builder.Append(Model.ReturnTypeName);
        builder.Append(" ");
        builder.Append(Model.ExplicitInterfaceName);
        builder.Append(Model.Name);
        builder.Append(Model.GenericTypeArguments);
        builder.Append("(");

        if (Model.ExtensionMethod)
        {
            builder.Append("this ");
        }

        RenderChildTemplatesByModel(Model.Parameters, builder);

        builder.Append(")");
        builder.Append(Model.GenericTypeArgumentConstraints);

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
