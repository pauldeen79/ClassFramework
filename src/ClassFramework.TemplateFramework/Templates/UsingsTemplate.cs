namespace ClassFramework.TemplateFramework.Templates;

public sealed class UsingsTemplate : CsharpClassGeneratorBase<UsingsViewModel>, IBuilderTemplate<StringBuilder>
{
    public Task<Result> RenderAsync(StringBuilder builder, CancellationToken token)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        var anyUsings = false;
        foreach (var @using in Model.Usings)
        {
            builder.AppendLine($"using {@using};");
            anyUsings = true;
        }

        if (anyUsings)
        {
            builder.AppendLine();
        }

        return Task.FromResult(Result.Success());
    }
}
