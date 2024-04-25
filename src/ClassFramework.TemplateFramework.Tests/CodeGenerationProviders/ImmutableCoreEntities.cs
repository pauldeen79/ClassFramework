﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableCoreEntities : ImmutableCSharpClassBase
{
    public ImmutableCoreEntities(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator) : base(csharpExpressionDumper, mediator)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetEntities(await GetCoreModels().ConfigureAwait(false), "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain";
}
