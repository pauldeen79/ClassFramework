namespace ClassFramework.CodeGeneration.Models.Abstractions;

internal interface IReferenceType : IType
{
    bool Static { get; }
    bool Sealed { get; }
    bool Abstract { get; }
}
