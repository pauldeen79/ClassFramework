namespace ClassFramework.TemplateFramework.Templates;

public class EnumerationMemberTemplate : CsharpClassGeneratorBase<EnumerationMemberViewModel>, IStringBuilderTemplate
{
    public Task Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        builder.Append(Model.CreateIndentation(2));
        builder.Append(Model.Name);
        builder.Append(Model.ValueExpression);
        builder.AppendLine(",");

        return Task.CompletedTask;
    }
}
