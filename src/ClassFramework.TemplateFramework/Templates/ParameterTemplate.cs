﻿namespace ClassFramework.TemplateFramework.Templates;

public class ParameterTemplate : CsharpClassGeneratorBase<ParameterViewModel>, IBuilderTemplate<StringBuilder>
{
    public async Task<Result> Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        return (await RenderChildTemplatesByModel(Model.Attributes, builder, cancellationToken).ConfigureAwait(false))
            .OnSuccess(_ =>
            {
                builder.Append(Model.Prefix);
                builder.Append(Model.TypeName);
                builder.Append(" ");
                builder.Append(Model.Name);

                if (Model.ShouldRenderDefaultValue)
                {
                    builder.Append(" = ");
                    builder.Append(Model.DefaultValueExpression);
                }
            });
    }
}
