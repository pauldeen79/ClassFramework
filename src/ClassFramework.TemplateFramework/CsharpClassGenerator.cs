﻿namespace ClassFramework.TemplateFramework;

public sealed class CsharpClassGenerator : CsharpClassGeneratorBase<CsharpClassGeneratorViewModel>, IMultipleContentBuilderTemplate, IBuilderTemplate<StringBuilder>
{
    public async Task<Result> Render(IMultipleContentBuilder<StringBuilder> builder, CancellationToken cancellationToken)
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
            await RenderHeader(generationEnvironment, cancellationToken).ConfigureAwait(false);
        }

        singleStringBuilder?.AppendLineWithCondition("#nullable enable", Model.ShouldRenderNullablePragmas);
        return (await RenderNamespaceHierarchy(generationEnvironment, singleStringBuilder, cancellationToken).ConfigureAwait(false))
            .OnSuccess(_ => singleStringBuilder?.AppendLineWithCondition("#nullable disable", Model.ShouldRenderNullablePragmas));
    }

    public async Task<Result> Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        if (Model.Settings.GenerateMultipleFiles)
        {
            throw new NotSupportedException("Can't generate multiple files, because the generation environment has a single StringBuilder instance");
        }

        var generationEnvironment = new StringBuilderEnvironment(builder);
        var result = await RenderHeader(generationEnvironment, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccessful())
        {
            return result;
        }

        generationEnvironment.Builder.AppendLineWithCondition("#nullable enable", Model.ShouldRenderNullablePragmas);
        return (await RenderNamespaceHierarchy(generationEnvironment, builder, cancellationToken).ConfigureAwait(false))
            .OnSuccess(_ => generationEnvironment.Builder.AppendLineWithCondition("#nullable disable", Model.ShouldRenderNullablePragmas));
    }

    private async Task<Result> RenderHeader(IGenerationEnvironment generationEnvironment, CancellationToken cancellationToken)
    {
        var result = await RenderChildTemplateByModel(Model!.GetCodeGenerationHeaderModel(), generationEnvironment, cancellationToken).ConfigureAwait(false);
        return await result.OnSuccess(async _ =>
        {
            if (!Model.Settings.EnableGlobalUsings)
            {
                return await RenderChildTemplateByModel(Model.Usings, generationEnvironment, cancellationToken).ConfigureAwait(false);
            }
            return Result.Success();
        });
    }

    private async Task<Result> RenderNamespaceHierarchy(IGenerationEnvironment generationEnvironment, StringBuilder? singleStringBuilder, CancellationToken cancellationToken)
    {
        foreach (var @namespace in Model!.Namespaces)
        {
            if (singleStringBuilder is not null && !string.IsNullOrEmpty(@namespace.Key))
            {
                singleStringBuilder.AppendLine($"namespace {@namespace.Key}");
                singleStringBuilder.AppendLine("{"); // open namespace
            }

            var result = await RenderChildTemplatesByModel(Model.GetTypes(@namespace), generationEnvironment, cancellationToken).ConfigureAwait(false);

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
    }}
