namespace ClassFramework.TemplateFramework.Templates;

public class FieldTemplate : CsharpClassGeneratorBase<FieldViewModel>, IBuilderTemplate<StringBuilder>
{
    public async Task<Result> Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        var result = await RenderChildTemplatesByModel(Model.Attributes, builder, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccessful())
        {
            return result;
        }

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

        return Result.Success();
    }
}
