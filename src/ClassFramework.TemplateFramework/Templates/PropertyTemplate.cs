namespace ClassFramework.TemplateFramework.Templates;

public class PropertyTemplate : CsharpClassGeneratorBase<PropertyViewModel>, IStringBuilderTemplate
{
    public void Render(StringBuilder builder)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        RenderChildTemplatesByModel(Model.Attributes, builder);

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

        RenderChildTemplatesByModel(Model.CodeBodyItems, builder);

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
