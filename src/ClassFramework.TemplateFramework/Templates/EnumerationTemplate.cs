namespace ClassFramework.TemplateFramework.Templates;

public class EnumerationTemplate : CsharpClassGeneratorBase<EnumerationViewModel>, IBuilderTemplate<StringBuilder>
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
        builder.Append("enum ");
        builder.AppendLine(Model.Name);
        builder.Append(Model.CreateIndentation(1));
        builder.AppendLine("{");

        return (await RenderChildTemplatesByModel(Model.Members, builder, cancellationToken).ConfigureAwait(false))
            .OnSuccess(_ =>
            {
                builder.Append(Model.CreateIndentation(1));
                builder.AppendLine("}");

            });
    }
}
