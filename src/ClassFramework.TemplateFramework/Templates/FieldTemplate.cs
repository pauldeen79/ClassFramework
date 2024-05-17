namespace ClassFramework.TemplateFramework.Templates;

public class FieldTemplate : CsharpClassGeneratorBase<FieldViewModel>, IStringBuilderTemplate
{
    public async Task Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        await RenderChildTemplatesByModel(Model.Attributes, builder, cancellationToken).ConfigureAwait(false);

        builder.Append(Model.CreateIndentation(1));
        builder.Append(Model.Modifiers);
        
        if (Model.Event)
        {
            builder.Append("event ");
        }
        
        builder.Append($"{Model.TypeName} {Model.Name}");
        
        if (Model.ShouldRenderDefaultValue)
        {
            builder.Append(" = ");
            builder.Append(Model.DefaultValueExpression);
        }
        
        builder.AppendLine(";");
    }
}
