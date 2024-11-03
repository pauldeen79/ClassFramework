namespace ClassFramework.CodeGeneration.Models.TemplateFramework;

internal interface ICsharpClassGeneratorSettings
{
    bool RecurseOnDeleteGeneratedFiles { get; }
    [Required(AllowEmptyStrings = true)] string LastGeneratedFilesFilename { get; }
    [Required] Encoding Encoding { get; }

    [Required(AllowEmptyStrings = true)] string Path { get; }
    [Required] CultureInfo CultureInfo { get; }
    bool GenerateMultipleFiles { get; }
    bool SkipWhenFileExists { get; }
    bool CreateCodeGenerationHeader { get; }
    [Required(AllowEmptyStrings = true)] string EnvironmentVersion { get; }
    [Required(AllowEmptyStrings = true)] string FilenameSuffix { get; }
    bool EnableNullableContext { get; }
    [DefaultValue(true)] bool EnableNullablePragmas { get; }
    bool EnableGlobalUsings { get; }
    [Required] IReadOnlyCollection<string> CustomUsings { get; }
    [Required] IReadOnlyCollection<string> NamespacesToAbbreviate { get; }
}
