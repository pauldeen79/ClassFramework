﻿namespace ClassFramework.TemplateFramework.Templates;

public class EnumerationTemplate : CsharpClassGeneratorBase<EnumerationViewModel>, IBuilderTemplate<StringBuilder>
{
    public async Task<Result> RenderAsync(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        return await (await RenderChildTemplatesByModelAsync(Model.Attributes, builder, cancellationToken).ConfigureAwait(false))
            .OnSuccess(async () =>
            {
                builder.Append(Model.CreateIndentation(1));
                builder.Append(Model.Modifiers);
                builder.Append("enum ");
                builder.AppendLine(Model.Name);
                builder.Append(Model.CreateIndentation(1));
                builder.AppendLine("{");

                return (await RenderChildTemplatesByModelAsync(Model.Members, builder, cancellationToken).ConfigureAwait(false))
                    .OnSuccess(() =>
                    {
                        builder.Append(Model.CreateIndentation(1));
                        builder.AppendLine("}");

                    });
            }).ConfigureAwait(false);
    }
}
