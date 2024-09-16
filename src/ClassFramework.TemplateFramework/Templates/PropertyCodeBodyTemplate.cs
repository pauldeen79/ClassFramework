namespace ClassFramework.TemplateFramework.Templates;

public class PropertyCodeBodyTemplate : CsharpClassGeneratorBase<PropertyCodeBodyViewModel>, IBuilderTemplate<StringBuilder>
{
    public async Task<Result> Render(StringBuilder builder, CancellationToken cancellationToken)
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
            var result = await RenderChildTemplatesByModel(Model.CodeStatements, builder, cancellationToken).ConfigureAwait(false);
            if (!result.IsSuccessful())
            {
                return result;
            }

            builder.Append(Model.CreateIndentation(2));
            builder.AppendLine("}");
        }

        return Result.Success();
    }
}
