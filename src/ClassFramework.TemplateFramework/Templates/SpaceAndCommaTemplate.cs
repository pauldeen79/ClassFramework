namespace ClassFramework.TemplateFramework.Templates;

public class SpaceAndCommaTemplate : CsharpClassGeneratorBase<SpaceAndCommaViewModel>, IBuilderTemplate<StringBuilder>
{
    public Task<Result> RenderAsync(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);

        builder.Append(", ");

        return Task.FromResult(Result.Success());
    }
}
