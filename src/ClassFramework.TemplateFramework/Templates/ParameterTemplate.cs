namespace ClassFramework.TemplateFramework.Templates;

public class ParameterTemplate : CsharpClassGeneratorBase<ParameterViewModel>, IStringBuilderTemplate
{
    public async Task Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        await RenderChildTemplatesByModel(Model.Attributes, builder, cancellationToken).ConfigureAwait(false);

        builder.Append(Model.Prefix);
        builder.Append(Model.TypeName);
        builder.Append(" ");
        builder.Append(Model.Name);

        if (Model.ShouldRenderDefaultValue)
        {
            builder.Append(" = ");
            builder.Append(Model.DefaultValueExpression);
        }
    }
}
