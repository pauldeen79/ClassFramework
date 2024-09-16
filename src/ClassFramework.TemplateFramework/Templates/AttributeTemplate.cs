namespace ClassFramework.TemplateFramework.Templates;

public sealed class AttributeTemplate : CsharpClassGeneratorBase<AttributeViewModel>, IBuilderTemplate<StringBuilder>
{
    public Task<Result> Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        if (!Model.IsSingleLineAttributeContainer)
        {
            builder.Append(Model.CreateIndentation(Model.AdditionalIndents));
        }

        builder.Append("[");
        builder.Append(Model.Name);
        builder.Append(Model.Parameters);
        builder.Append("]");

        if (!Model.IsSingleLineAttributeContainer)
        {
            builder.AppendLine();
        }
        else
        {
            builder.Append(" ");
        }

        return Task.FromResult(Result.Success());
    }
}
