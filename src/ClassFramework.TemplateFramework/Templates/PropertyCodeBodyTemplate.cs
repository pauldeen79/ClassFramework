namespace ClassFramework.TemplateFramework.Templates;

public class PropertyCodeBodyTemplate : CsharpClassGeneratorBase<PropertyCodeBodyViewModel>, IBuilderTemplate<StringBuilder>
{
    public async Task<Result> RenderAsync(StringBuilder builder, CancellationToken token)
    {
        Guard.IsNotNull(Model);
        Guard.IsNotNull(builder);

        builder.Append(Model.CreateIndentation(2));
        builder.Append(Model.Modifiers);
        builder.Append(Model.Verb);
        if (Model.OmitCode)
        {
            builder.AppendLine(";");
            return Result.Success();
        }
        else
        {
            builder.AppendLine();
            builder.Append(Model.CreateIndentation(2));
            builder.AppendLine("{");
            return (await RenderChildTemplatesByModelAsync(Model.CodeStatements, builder, token).ConfigureAwait(false))
                .OnSuccess(() =>
                {
                    builder.Append(Model.CreateIndentation(2));
                    builder.AppendLine("}");
                });
        }
    }
}
