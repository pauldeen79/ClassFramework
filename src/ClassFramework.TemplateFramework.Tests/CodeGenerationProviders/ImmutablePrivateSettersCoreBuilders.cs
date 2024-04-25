﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutablePrivateSettersCoreBuilders : ImmutablePrivateSettersCSharpClassBase
{
    public ImmutablePrivateSettersCoreBuilders(ICsharpExpressionDumper csharpExpressionDumper, IMediator mediator) : base(csharpExpressionDumper, mediator)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilders(await GetCoreModels().ConfigureAwait(false), "Test.Domain.Builders", "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders";
}
