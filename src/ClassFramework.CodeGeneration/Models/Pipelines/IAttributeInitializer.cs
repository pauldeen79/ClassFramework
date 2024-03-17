namespace ClassFramework.CodeGeneration.Models.Pipelines;

internal interface IAttributeInitializer
{
    Func<System.Attribute, IAttribute?> Result { get; }
}
