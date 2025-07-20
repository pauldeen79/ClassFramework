namespace ClassFramework.CodeGeneration.Models.Test;

internal interface ITestEntity
{
    string SingleProperty { get; }
    [Required] IReadOnlyCollection<string> CollectionProperty { get; }
}
