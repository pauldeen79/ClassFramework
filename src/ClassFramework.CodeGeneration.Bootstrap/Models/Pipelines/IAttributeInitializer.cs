namespace ClassFramework.CodeGeneration.Bootstrap.Models.Pipelines;

internal interface IAttributeInitializer
{
    Func<Attribute, IAttribute?> Result { get; }
}
