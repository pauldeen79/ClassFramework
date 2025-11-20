namespace ClassFramework.TemplateFramework.Templates;

public class ParameterTemplate : CsharpClassGeneratorBase<ParameterViewModel>, IBuilderTemplate<StringBuilder>
{
    public async Task<Result> RenderAsync(StringBuilder builder, CancellationToken token)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        return (await RenderChildTemplatesByModelAsync(Model.Attributes, builder, token).ConfigureAwait(false))
            .OnSuccess(() =>
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
