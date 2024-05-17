namespace ClassFramework.TemplateFramework.Templates;

public class SpaceAndCommaTemplate : CsharpClassGeneratorBase<SpaceAndCommaViewModel>, IStringBuilderTemplate
{
    public Task Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);

        builder.Append(", ");

        return Task.CompletedTask;
    }
}
