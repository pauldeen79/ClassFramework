namespace ClassFramework.TemplateFramework.Templates;

public class NewLineTemplate : CsharpClassGeneratorBase<NewLineViewModel>, IStringBuilderTemplate
{
    public Task Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);

        builder.AppendLine();

        return Task.CompletedTask;
    }
}
