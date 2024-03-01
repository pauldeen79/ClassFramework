namespace ClassFramework.TemplateFramework.Templates;

public class MethodTemplate : CsharpClassGeneratorBase<MethodViewModel>, IStringBuilderTemplate
{
    public void Render(StringBuilder builder)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        RenderChildTemplatesByModel(Model.GetAttributeModels(), builder);

        if (!Model.OmitCode)
        {
            foreach (var suppression in Model.SuppressWarningCodes)
            {
                builder.Append(Model.CreateIndentation(1));
                builder.AppendLine($"#pragma warning disable {suppression}");
            }
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

        RenderChildTemplatesByModel(Model.GetParameterModels(), builder);

        builder.Append(")");
        builder.Append(Model.GenericTypeArgumentConstraints);

        if (Model.OmitCode)
        {
            builder.AppendLine(";");
        }
        else
        {
            builder.AppendLine();
            builder.Append(Model.CreateIndentation(1));
            builder.AppendLine("{");
            RenderChildTemplatesByModel(Model.GetCodeStatementModels(), builder);
            builder.Append(Model.CreateIndentation(1));
            builder.AppendLine("}");

            foreach (var suppression in Model.SuppressWarningCodes)
            {
                builder.Append(Model.CreateIndentation(1));
                builder.AppendLine($"#pragma warning disable {suppression}");
            }
        }
    }
}
