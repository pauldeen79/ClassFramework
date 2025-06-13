namespace ClassFramework.TemplateFramework.Templates;

public class NewLineTemplate : CsharpClassGeneratorBase<NewLineViewModel>, IBuilderTemplate<StringBuilder>
{
    public Task<Result> RenderAsync(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);

        builder.AppendLine();

        return Task.FromResult(Result.Success());
    }
}
