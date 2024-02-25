namespace ClassFramework.IntegrationTests.Models;

internal interface IMetadata : Abstractions.INameContainer
{
    object? Value { get; }
}
