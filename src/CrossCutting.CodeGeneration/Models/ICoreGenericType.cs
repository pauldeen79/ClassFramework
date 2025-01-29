namespace CrossCutting.CodeGeneration.Models;

internal interface ICoreGenericType<T>
{
    T MyProperty { get; }
}
