﻿namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class CoreBuilders : ClassFrameworkCSharpClassBase
{
    public CoreBuilders(IMediator mediator) : base(mediator)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilders(await GetCoreModels().ConfigureAwait(false), "ClassFramework.Domain.Builders", "ClassFramework.Domain").ConfigureAwait(false);

    public override string Path => "ClassFramework.Domain/Builders";

    protected override bool CreateAsObservable => true;
}
