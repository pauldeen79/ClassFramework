namespace ClassFramework.TemplateFramework.Tests.Models.Overrides;

internal interface IMyGenericOverride<out T> : IAbstractBase
{
    T MyOverrideProperty { get; }
}
