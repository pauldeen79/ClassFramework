namespace ClassFramework.TemplateFramework.Templates;

public sealed class TypeTemplate : CsharpClassGeneratorBase<TypeViewModel>, IMultipleContentBuilderTemplate, IStringBuilderTemplate
{
    public async Task Render(IMultipleContentBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);
        Guard.IsNotNull(Context);

        StringBuilderEnvironment generationEnvironment;

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
            await RenderChildTemplateByModel(Model.CodeGenerationHeaders, generationEnvironment, cancellationToken).ConfigureAwait(false);
            if (!Model.Settings.EnableGlobalUsings)
            {
                await RenderChildTemplateByModel(Model.Usings(), generationEnvironment, cancellationToken).ConfigureAwait(false);
            }

            if (Model.ShouldRenderNamespaceScope)
            {
                generationEnvironment.Builder.AppendLine($"namespace {Model.Namespace}");
                generationEnvironment.Builder.AppendLine("{"); // start namespace
            }
        }

        await RenderTypeBase(generationEnvironment, cancellationToken).ConfigureAwait(false);

        generationEnvironment.Builder.AppendLineWithCondition("}", Model.ShouldRenderNamespaceScope); // end namespace
    }

    public async Task Render(StringBuilder builder, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(Model);

        var generationEnvironment = new StringBuilderEnvironment(builder);
        await RenderTypeBase(generationEnvironment, cancellationToken).ConfigureAwait(false);
    }

    private async Task RenderTypeBase(StringBuilderEnvironment generationEnvironment, CancellationToken cancellationToken)
    {
        generationEnvironment.Builder.AppendLineWithCondition("#nullable enable", Model!.ShouldRenderNullablePragmas);

        foreach (var suppression in Model.SuppressWarningCodes)
        {
            generationEnvironment.Builder.AppendLine($"#pragma warning disable {suppression}");
        }

        var indentedBuilder = new IndentedStringBuilder(generationEnvironment.Builder);
        PushIndent(indentedBuilder);

        await RenderChildTemplatesByModel(Model.Attributes, generationEnvironment, cancellationToken).ConfigureAwait(false);

        indentedBuilder.AppendLine($"{Model.Modifiers}{Model.ContainerType} {Model.Name}{Model.GenericTypeArguments}{Model.InheritedClasses}{Model.GenericTypeArgumentConstraints}");
        indentedBuilder.AppendLine("{"); // start class

        // Fields, Properties, Methods, Constructors, Enumerations
        await RenderChildTemplatesByModel(Model.Members, generationEnvironment, cancellationToken).ConfigureAwait(false);

        // Subclasses
        await RenderChildTemplatesByModel(Model.SubClasses, generationEnvironment, cancellationToken).ConfigureAwait(false);

        indentedBuilder.AppendLine("}"); // end class

        PopIndent(indentedBuilder);

        generationEnvironment.Builder.AppendLineWithCondition("#nullable restore", Model.ShouldRenderNullablePragmas);

        foreach (var suppression in Model.SuppressWarningCodes.Reverse())
        {
            generationEnvironment.Builder.AppendLine($"#pragma warning restore {suppression}");
        }
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
