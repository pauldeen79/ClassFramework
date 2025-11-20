namespace ClassFramework.TemplateFramework.Templates;

public class NewLineTemplate : CsharpClassGeneratorBase<NewLineViewModel>, IBuilderTemplate<StringBuilder>
{
    public Task<Result> RenderAsync(StringBuilder builder, CancellationToken token)
    {
        Guard.IsNotNull(builder);

        builder.AppendLine();

        return Task.FromResult(Result.Success());
    }
}
