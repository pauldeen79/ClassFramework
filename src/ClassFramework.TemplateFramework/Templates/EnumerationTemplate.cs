namespace ClassFramework.TemplateFramework.Templates;

public class EnumerationTemplate : CsharpClassGeneratorBase<EnumerationViewModel>, IStringBuilderTemplate
{
    public async Task Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        await RenderChildTemplatesByModel(Model.Attributes, builder, cancellationToken).ConfigureAwait(false);

        builder.Append(Model.CreateIndentation(1));
        builder.Append(Model.Modifiers);
        builder.Append("enum ");
        builder.AppendLine(Model.Name);
        builder.Append(Model.CreateIndentation(1));
        builder.AppendLine("{");

        await RenderChildTemplatesByModel(Model.Members, builder, cancellationToken).ConfigureAwait(false);

        builder.Append(Model.CreateIndentation(1));
        builder.AppendLine("}");
    }
}
