namespace ClassFramework.TemplateFramework.Templates;

public class ConstructorTemplate : CsharpClassGeneratorBase<ConstructorViewModel>, IStringBuilderTemplate
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
        builder.Append(Model.Modifiers);
        builder.Append(Model.Name);
        builder.Append("(");

        await RenderChildTemplatesByModel(Model.Parameters, builder, cancellationToken).ConfigureAwait(false);

        builder.Append(")");
        builder.Append(Model.ChainCall);

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
