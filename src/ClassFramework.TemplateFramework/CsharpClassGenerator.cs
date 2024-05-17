namespace ClassFramework.TemplateFramework;

public sealed class CsharpClassGenerator : CsharpClassGeneratorBase<CsharpClassGeneratorViewModel>, IMultipleContentBuilderTemplate, IStringBuilderTemplate
{
    public async Task Render(IMultipleContentBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);
        Guard.IsNotNull(Context);

        StringBuilder? singleStringBuilder = null;
        IGenerationEnvironment generationEnvironment = new MultipleContentBuilderEnvironment(builder);

        if (!Model.Settings.GenerateMultipleFiles)
        {
            // Generate a single generation environment, so we create only a single file in the multiple content builder environment.
            singleStringBuilder = builder.AddContent(Context.DefaultFilename, Model.Settings.SkipWhenFileExists).Builder;
            generationEnvironment = new StringBuilderEnvironment(singleStringBuilder);
            await RenderHeader(generationEnvironment, cancellationToken).ConfigureAwait(false);
        }

        singleStringBuilder?.AppendLineWithCondition("#nullable enable", Model.ShouldRenderNullablePragmas);
        await RenderNamespaceHierarchy(generationEnvironment, singleStringBuilder, cancellationToken).ConfigureAwait(false);
        singleStringBuilder?.AppendLineWithCondition("#nullable disable", Model.ShouldRenderNullablePragmas);
    }

    public async Task Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        if (Model.Settings.GenerateMultipleFiles)
        {
            throw new NotSupportedException("Can't generate multiple files, because the generation environment has a single StringBuilder instance");
        }

        var generationEnvironment = new StringBuilderEnvironment(builder);
        await RenderHeader(generationEnvironment, cancellationToken).ConfigureAwait(false);
        generationEnvironment.Builder.AppendLineWithCondition("#nullable enable", Model.ShouldRenderNullablePragmas);
        await RenderNamespaceHierarchy(generationEnvironment, builder, cancellationToken);
        generationEnvironment.Builder.AppendLineWithCondition("#nullable disable", Model.ShouldRenderNullablePragmas);
    }

    private async Task RenderHeader(IGenerationEnvironment generationEnvironment, CancellationToken cancellationToken)
    {
        await RenderChildTemplateByModel(Model!.GetCodeGenerationHeaderModel(), generationEnvironment, cancellationToken);
        if (!Model.Settings.EnableGlobalUsings)
        {
            await RenderChildTemplateByModel(Model.Usings, generationEnvironment, cancellationToken);
        }
    }

    private async Task RenderNamespaceHierarchy(IGenerationEnvironment generationEnvironment, StringBuilder? singleStringBuilder, CancellationToken cancellationToken)
    {
        foreach (var @namespace in Model!.Namespaces)
        {
            if (singleStringBuilder is not null && !string.IsNullOrEmpty(@namespace.Key))
            {
                singleStringBuilder.AppendLine($"namespace {@namespace.Key}");
                singleStringBuilder.AppendLine("{"); // open namespace
            }

            await RenderChildTemplatesByModel(Model.GetTypes(@namespace), generationEnvironment, cancellationToken);

            if (singleStringBuilder is not null && !string.IsNullOrEmpty(@namespace.Key))
            {
                singleStringBuilder.AppendLine("}"); // close namespace
            }
        }
    }}
