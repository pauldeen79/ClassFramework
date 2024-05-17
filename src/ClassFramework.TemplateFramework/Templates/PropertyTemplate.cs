namespace ClassFramework.TemplateFramework.Templates;

public class PropertyTemplate : CsharpClassGeneratorBase<PropertyViewModel>, IStringBuilderTemplate
{
    public async Task Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        await RenderChildTemplatesByModel(Model.Attributes, builder, cancellationToken).ConfigureAwait(false);

        builder.Append(Model.CreateIndentation(1));

        if (Model.ShouldRenderModifiers)
        {
            builder.Append(Model.Modifiers);
        }

        builder.Append(Model.ExplicitInterfaceName);
        builder.Append(Model.TypeName);
        builder.Append(" ");
        builder.AppendLine(Model.Name);

        builder.Append(Model.CreateIndentation(1));
        builder.AppendLine("{");

        await RenderChildTemplatesByModel(Model.CodeBodyItems, builder, cancellationToken).ConfigureAwait(false);

        builder.Append(Model.CreateIndentation(1));
        builder.Append("}");

        if (Model.ShouldRenderDefaultValue)
        {
            builder.Append(" = ");
            builder.Append(Model.DefaultValueExpression);
            builder.Append(";");
        }

        builder.AppendLine();
    }
}
