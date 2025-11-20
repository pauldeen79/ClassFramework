namespace ClassFramework.TemplateFramework.Templates;

public class PropertyTemplate : CsharpClassGeneratorBase<PropertyViewModel>, IBuilderTemplate<StringBuilder>
{
    public async Task<Result> RenderAsync(StringBuilder builder, CancellationToken token)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        return await (await RenderChildTemplatesByModelAsync(Model.Attributes, builder, token).ConfigureAwait(false))
            .OnSuccessAsync(async () =>
            {
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

                return (await RenderChildTemplatesByModelAsync(Model.CodeBodyItems, builder, token).ConfigureAwait(false))
                    .OnSuccess(() =>
                    {
                        builder.Append(Model.CreateIndentation(1));
                        builder.Append("}");

                        if (Model.ShouldRenderDefaultValue)
                        {
                            builder.Append(" = ");
                            builder.Append(Model.DefaultValueExpression);
                            builder.Append(";");
                        }

                        builder.AppendLine();
                    });
            }).ConfigureAwait(false);
    }
}
