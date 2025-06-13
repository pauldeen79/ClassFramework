namespace ClassFramework.TemplateFramework.Templates;

public class ConstructorTemplate : CsharpClassGeneratorBase<ConstructorViewModel>, IBuilderTemplate<StringBuilder>
{
    public async Task<Result> RenderAsync(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        var result = await RenderChildTemplatesByModel(Model.Attributes, builder, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccessful())
        {
            return result;
        }

        if (!Model.OmitCode)
        {
            builder.RenderSuppressions(Model.SuppressWarningCodes, "disable", Model.CreateIndentation(1));
        }

        builder.Append(Model.CreateIndentation(1));
        builder.Append(Model.Modifiers);
        builder.Append(Model.Name);
        builder.Append("(");

        result = await RenderChildTemplatesByModel(Model.Parameters, builder, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccessful())
        {
            return result;
        }

        builder.Append(")");
        builder.Append(Model.ChainCall);

        if (Model.OmitCode)
        {
            builder.AppendLine(";");
        }
        else
        {
            result = await builder.RenderMethodBody(Model.CreateIndentation(1), () => RenderChildTemplatesByModel(Model.CodeStatements, builder, cancellationToken)).ConfigureAwait(false);
            if (!result.IsSuccessful())
            {
                return result;
            }

            builder.RenderSuppressions(Model.SuppressWarningCodes, "restore", Model.CreateIndentation(1));
        }

        return Result.Success();
    }
}
