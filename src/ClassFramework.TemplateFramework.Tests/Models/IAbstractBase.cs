namespace ClassFramework.TemplateFramework.Tests.Models;

internal interface IAbstractBase
{
    string MyBaseProperty { get; }
}

#pragma warning disable S2326 // Unused type parameters should be removed
internal interface IAbstractBase<T>
#pragma warning restore S2326 // Unused type parameters should be removed
{
}
