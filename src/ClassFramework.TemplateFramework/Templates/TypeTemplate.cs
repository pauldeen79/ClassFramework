namespace ClassFramework.TemplateFramework.Templates;

public sealed class TypeTemplate : CsharpClassGeneratorBase<TypeViewModel>, IMultipleContentBuilderTemplate, IBuilderTemplate<StringBuilder>
{
    public async Task<Result> Render(IMultipleContentBuilder<StringBuilder> builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);
        Guard.IsNotNull(Context);

        StringBuilderEnvironment generationEnvironment;
        Result result;

        if (!Model.Settings.GenerateMultipleFiles)
        {
            if (!builder.Contents.Any())
            {
                builder.AddContent(Context.DefaultFilename, Model.Settings.SkipWhenFileExists);
            }

            generationEnvironment = new StringBuilderEnvironment(builder.Contents.Last().Builder); // important to take the last contents, in case of sub classes
        }
        else
        {
            var filename = $"{Model.FilenamePrefix}{Model.Name}{Model.Settings.FilenameSuffix}.cs";
            var contentBuilder = builder.AddContent(filename, Model.Settings.SkipWhenFileExists);
            generationEnvironment = new StringBuilderEnvironment(contentBuilder.Builder);
            result = await RenderChildTemplateByModel(Model.CodeGenerationHeaders, generationEnvironment, cancellationToken).ConfigureAwait(false);
            if (!result.IsSuccessful())
            {
                return result;
            }

            if (!Model.Settings.EnableGlobalUsings)
            {
                result = await RenderChildTemplateByModel(Model.Usings(), generationEnvironment, cancellationToken).ConfigureAwait(false);
                if (!result.IsSuccessful())
                {
                    return result;
                }
            }

            if (Model.ShouldRenderNamespaceScope)
            {
                generationEnvironment.Builder.AppendLine($"namespace {Model.Namespace}");
                generationEnvironment.Builder.AppendLine("{"); // start namespace
            }
        }

        return (await RenderTypeBase(generationEnvironment, cancellationToken).ConfigureAwait(false))
            .OnSuccess(_ => generationEnvironment.Builder.AppendLineWithCondition("}", Model.ShouldRenderNamespaceScope)); // end namespace
    }

    public async Task<Result> Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        var generationEnvironment = new StringBuilderEnvironment(builder);
        return await RenderTypeBase(generationEnvironment, cancellationToken).ConfigureAwait(false);
    }

    private async Task<Result> RenderTypeBase(StringBuilderEnvironment generationEnvironment, CancellationToken cancellationToken)
    {
        generationEnvironment.Builder.AppendLineWithCondition("#nullable enable", Model!.ShouldRenderNullablePragmas);

        foreach (var suppression in Model.SuppressWarningCodes)
        {
            generationEnvironment.Builder.AppendLine($"#pragma warning disable {suppression}");
        }

        var indentedBuilder = new IndentedStringBuilder(generationEnvironment.Builder);
        PushIndent(indentedBuilder);

        var result = await RenderChildTemplatesByModel(Model.Attributes, generationEnvironment, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccessful())
        {
            return result;
        }

        indentedBuilder.AppendLine($"{Model.Modifiers}{Model.ContainerType} {Model.Name}{Model.GenericTypeArguments}{Model.InheritedClasses}{Model.GenericTypeArgumentConstraints}");
        indentedBuilder.AppendLine("{"); // start class

        // Fields, Properties, Methods, Constructors, Enumerations
        result = await RenderChildTemplatesByModel(Model.Members, generationEnvironment, cancellationToken).ConfigureAwait(false);
        if (!result.IsSuccessful())
        {
            return result;
        }

        // Subclasses
        return (await RenderChildTemplatesByModel(Model.SubClasses, generationEnvironment, cancellationToken).ConfigureAwait(false))
            .OnSuccess(_ =>
            {
                indentedBuilder.AppendLine("}"); // end class

                PopIndent(indentedBuilder);

                generationEnvironment.Builder.AppendLineWithCondition("#nullable restore", Model.ShouldRenderNullablePragmas);

                foreach (var suppression in Model.SuppressWarningCodes.Reverse())
                {
                    generationEnvironment.Builder.AppendLine($"#pragma warning restore {suppression}");
                }
            });
    }

    private void PushIndent(IndentedStringBuilder indentedBuilder)
    {
        for (int i = 0; i < Context.GetIndentCount(); i++)
        {
            indentedBuilder.IncrementIndent();
        }
    }

    private void PopIndent(IndentedStringBuilder indentedBuilder)
    {
        for (int i = 0; i < Context.GetIndentCount(); i++)
        {
            indentedBuilder.DecrementIndent();
        }
    }
}
