namespace ClassFramework.TemplateFramework.Templates;

public class ParameterTemplate : CsharpClassGeneratorBase<ParameterViewModel>, IStringBuilderTemplate
{
    public void Render(StringBuilder builder)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        RenderChildTemplatesByModel(Model.Attributes, builder);

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
