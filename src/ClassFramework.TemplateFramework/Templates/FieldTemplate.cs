namespace ClassFramework.TemplateFramework.Templates;

public class FieldTemplate : CsharpClassGeneratorBase<FieldViewModel>, IBuilderTemplate<StringBuilder>
{
    public async Task<Result> RenderAsync(StringBuilder builder, CancellationToken token)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        return (await RenderChildTemplatesByModelAsync(Model.Attributes, builder, token).ConfigureAwait(false))
            .OnSuccess(() =>
            {

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
            });
    }
}
