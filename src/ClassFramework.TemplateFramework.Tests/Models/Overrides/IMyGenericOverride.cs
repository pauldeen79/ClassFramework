namespace ClassFramework.TemplateFramework.Tests.Models.Overrides;

internal interface IMyGenericOverride<T> : IAbstractBase<T>
{
    T MyOverrideProperty { get; }
}
