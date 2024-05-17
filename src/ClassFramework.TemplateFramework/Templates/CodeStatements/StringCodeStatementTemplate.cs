namespace ClassFramework.TemplateFramework.Templates.CodeStatements;

public class StringCodeStatementTemplate : CsharpClassGeneratorBase<StringCodeStatementViewModel>, IStringBuilderTemplate
{
    public Task Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        builder.Append(Model.CreateIndentation(Model.AdditionalIndents));

        builder.AppendLine(Model.Statement);

        return Task.CompletedTask;
    }
}
