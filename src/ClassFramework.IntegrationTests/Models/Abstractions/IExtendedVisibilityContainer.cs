namespace ClassFramework.IntegrationTests.Models.Abstractions;

internal interface IExtendedVisibilityContainer : IVisibilityContainer
{
    bool Static { get; }
    bool Virtual { get; }
    bool Abstract { get; }
    bool Protected { get; }
    bool Override { get; }
}
