﻿namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces : ImmutableInheritFromInterfacesCSharpClassBase
{
    public ImmutableInheritFromInterfacesAbstractionsBuilderInterfaces(IMediator mediator) : base(mediator)
    {
    }

    public override async Task<IEnumerable<TypeBase>> GetModel() => await GetBuilderInterfaces(await GetCoreModels().ConfigureAwait(false), "Test.Domain.Builders", "Test.Domain", "Test.Abstractions").ConfigureAwait(false);

    public override string Path => "Test.Domain";
}
