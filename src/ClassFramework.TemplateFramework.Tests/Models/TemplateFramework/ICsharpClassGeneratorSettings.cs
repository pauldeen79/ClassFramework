﻿namespace ClassFramework.TemplateFramework.Tests.Models.TemplateFramework;

internal interface ICsharpClassGeneratorSettings
{
    bool RecurseOnDeleteGeneratedFiles { get; }
    [Required(AllowEmptyStrings = true)] string LastGeneratedFilesFilename { get; }
    [Required] Encoding Encoding { get; }

    [Required(AllowEmptyStrings = true)] string Path { get; }
    [Required] CultureInfo CultureInfo { get; }
    [DefaultValue(true)] bool GenerateMultipleFiles { get; }
    bool SkipWhenFileExists { get; }
    bool CreateCodeGenerationHeader { get; }
    [Required(AllowEmptyStrings = true)] string EnvironmentVersion { get; }
    [Required(AllowEmptyStrings = true)] string FilenameSuffix { get; }
    bool EnableNullableContext { get; }
    bool EnableGlobalUsings { get; }
}
