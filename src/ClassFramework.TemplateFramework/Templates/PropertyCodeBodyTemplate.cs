namespace ClassFramework.TemplateFramework.Templates;

public class PropertyCodeBodyTemplate : CsharpClassGeneratorBase<PropertyCodeBodyViewModel>, IStringBuilderTemplate
{
    public async Task Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(Model);
        Guard.IsNotNull(builder);

        builder.Append(Model.CreateIndentation(2));
        builder.Append(Model.Modifiers);
        builder.Append(Model.Verb);
        if (Model.OmitCode)
        {
            builder.AppendLine(";");
        }
        else
        {
            builder.AppendLine();
            builder.Append(Model.CreateIndentation(2));
            builder.AppendLine("{");
            await RenderChildTemplatesByModel(Model.CodeStatements, builder, cancellationToken).ConfigureAwait(false);
            builder.Append(Model.CreateIndentation(2));
            builder.AppendLine("}");
        }
    }
}
