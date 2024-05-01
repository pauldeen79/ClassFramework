﻿namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractionsBuildersExtensions : ClassFrameworkCSharpClassBase
{
    public AbstractionsBuildersExtensions(IMediator mediator, ICsharpExpressionDumper csharpExpressionDumper) : base(mediator, csharpExpressionDumper)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilderExtensions(await GetAbstractionsInterfaces().ConfigureAwait(false), "ClassFramework.Domain.Builders.Abstractions", "ClassFramework.Domain.Abstractions", "ClassFramework.Domain.Builders.Extensions").ConfigureAwait(false);

    public override string Path => "ClassFramework.Domain/Builders/Extensions";

    protected override bool EnableEntityInheritance => true;
}
