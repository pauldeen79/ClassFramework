namespace ClassFramework.TemplateFramework.Tests.Models;

internal interface IAbstractBase
{
    string MyBaseProperty { get; }
}

internal interface IAbstractBase<T>
{
}
