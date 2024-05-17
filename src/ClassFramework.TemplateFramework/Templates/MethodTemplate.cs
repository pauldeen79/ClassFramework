namespace ClassFramework.TemplateFramework.Templates;

public class MethodTemplate : CsharpClassGeneratorBase<MethodViewModel>, IStringBuilderTemplate
{
    public async Task Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        await RenderChildTemplatesByModel(Model.Attributes, builder, cancellationToken).ConfigureAwait(false);

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

        await RenderChildTemplatesByModel(Model.Parameters, builder, cancellationToken).ConfigureAwait(false);

        builder.Append(")");
        builder.Append(Model.GenericTypeArgumentConstraints);

        if (Model.OmitCode)
        {
            builder.AppendLine(";");
        }
        else
        {
            builder.RenderMethodBody(Model.CreateIndentation(1), async () => await RenderChildTemplatesByModel(Model.CodeStatements, builder, cancellationToken).ConfigureAwait(false));
            builder.RenderSuppressions(Model.SuppressWarningCodes, "restore", Model.CreateIndentation(1));
        }
    }
}
