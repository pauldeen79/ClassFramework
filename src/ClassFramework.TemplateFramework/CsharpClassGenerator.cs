namespace ClassFramework.TemplateFramework;

public sealed class CsharpClassGenerator : CsharpClassGeneratorBase<CsharpClassGeneratorViewModel>, IMultipleContentBuilderTemplate, IBuilderTemplate<StringBuilder>
{
    public async Task<Result> RenderAsync(IMultipleContentBuilder<StringBuilder> builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);
        Guard.IsNotNull(Context);

        StringBuilder? singleStringBuilder = null;
        IGenerationEnvironment generationEnvironment = new MultipleStringContentBuilderEnvironment(builder);

        if (!Model.Settings.GenerateMultipleFiles)
        {
            // Generate a single generation environment, so we create only a single file in the multiple content builder environment.
            singleStringBuilder = builder.AddContent(Context.DefaultFilename, Model.Settings.SkipWhenFileExists).Builder;
            generationEnvironment = new StringBuilderEnvironment(singleStringBuilder);
            var result = await RenderHeaderAsync(generationEnvironment, cancellationToken).ConfigureAwait(false);
            if (!result.IsSuccessful())
            {
                return result;
            }
        }

        singleStringBuilder?.AppendLineWithCondition("#nullable enable", Model.ShouldRenderNullablePragmas);
        return (await RenderNamespaceHierarchyAsync(generationEnvironment, singleStringBuilder, cancellationToken).ConfigureAwait(false))
            .OnSuccess(() => singleStringBuilder?.AppendLineWithCondition("#nullable disable", Model.ShouldRenderNullablePragmas));
    }

    public async Task<Result> RenderAsync(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        if (Model.Settings.GenerateMultipleFiles)
        {
            return Result.NotSupported("Can't generate multiple files, because the generation environment has a single StringBuilder instance");
        }

        var generationEnvironment = new StringBuilderEnvironment(builder);
        return await (await RenderHeaderAsync(generationEnvironment, cancellationToken).ConfigureAwait(false))
            .OnSuccessAsync(async () =>
            {
                generationEnvironment.Builder.AppendLineWithCondition("#nullable enable", Model.ShouldRenderNullablePragmas);
                return (await RenderNamespaceHierarchyAsync(generationEnvironment, builder, cancellationToken).ConfigureAwait(false))
                    .OnSuccess(() => generationEnvironment.Builder.AppendLineWithCondition("#nullable disable", Model.ShouldRenderNullablePragmas));
            }).ConfigureAwait(false);
    }

    private async Task<Result> RenderHeaderAsync(IGenerationEnvironment generationEnvironment, CancellationToken token)
    {
        return await (await RenderChildTemplateByModelAsync(Model.CodeGenerationHeaderModel, generationEnvironment, token).ConfigureAwait(false))
            .OnSuccessAsync(async () =>
            {
                if (!Model.Settings.EnableGlobalUsings)
                {
                    return await RenderChildTemplateByModelAsync(Model.Usings, generationEnvironment, token).ConfigureAwait(false);
                }
                return Result.Success();
            }).ConfigureAwait(false);
    }

    private async Task<Result> RenderNamespaceHierarchyAsync(IGenerationEnvironment generationEnvironment, StringBuilder? singleStringBuilder, CancellationToken token)
    {
        foreach (var @namespace in Model.Namespaces)
        {
            if (singleStringBuilder is not null && !string.IsNullOrEmpty(@namespace.Key))
            {
                singleStringBuilder.AppendLine($"namespace {@namespace.Key}");
                singleStringBuilder.AppendLine("{"); // open namespace
            }

            var result = await RenderChildTemplatesByModelAsync(Model.GetTypes(@namespace), generationEnvironment, token).ConfigureAwait(false);

            if (!result.IsSuccessful())
            {
                return result;
            }

            if (singleStringBuilder is not null && !string.IsNullOrEmpty(@namespace.Key))
            {
                singleStringBuilder.AppendLine("}"); // close namespace
            }
        }

        return Result.Success();
    }
}
