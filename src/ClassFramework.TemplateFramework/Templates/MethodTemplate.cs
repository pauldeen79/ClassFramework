namespace ClassFramework.TemplateFramework.Templates;

public class MethodTemplate : CsharpClassGeneratorBase<MethodViewModel>, IBuilderTemplate<StringBuilder>
{
    public async Task<Result> RenderAsync(StringBuilder builder, CancellationToken token)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        var result = await RenderChildTemplatesByModelAsync(Model.Attributes, builder, token).ConfigureAwait(false);
        if (!result.IsSuccessful())
        {
            return result;
        }

        if (!Model.OmitCode)
        {
            builder.RenderSuppressions(Model.SuppressWarningCodes, "disable", Model.CreateIndentation(1));
        }

        builder.Append(Model.CreateIndentation(1));

        if (Model.ShouldRenderModifiers)
        {
            builder.Append(Model.Modifiers);
        }

        builder.Append(Model.ReturnTypeName);
        builder.Append(Model.ReturnTypeGenericTypeArguments);
        builder.Append(" ");
        builder.Append(Model.ExplicitInterfaceName);
        builder.Append(Model.Name);
        builder.Append(Model.GenericTypeArguments);
        builder.Append("(");

        if (Model.ExtensionMethod)
        {
            builder.Append("this ");
        }

        result = await RenderChildTemplatesByModelAsync(Model.Parameters, builder, token).ConfigureAwait(false);
        if (!result.IsSuccessful())
        {
            return result;
        }

        builder.Append(")");
        builder.Append(Model.GenericTypeArgumentConstraints);

        if (Model.OmitCode)
        {
            builder.AppendLine(";");
        }
        else
        {
            result = await builder.RenderMethodBody(Model.CreateIndentation(1), () => RenderChildTemplatesByModelAsync(Model.CodeStatements, builder, token)).ConfigureAwait(false);
            if (!result.IsSuccessful())
            {
                return result;
            }

            builder.RenderSuppressions(Model.SuppressWarningCodes, "restore", Model.CreateIndentation(1));
        }

        return Result.Success();
    }
}
