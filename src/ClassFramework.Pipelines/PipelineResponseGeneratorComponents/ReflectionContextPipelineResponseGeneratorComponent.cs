namespace ClassFramework.Pipelines.PipelineResponseGeneratorComponents;

public class ReflectionContextPipelineResponseGeneratorComponent : IPipelineResponseGeneratorComponent
{
    public Result<T> Generate<T>(object command)
    {
        if (command is Reflection.ReflectionContext reflectionContext && typeof(TypeBaseBuilder).IsAssignableFrom(typeof(T)))
        {
            TypeBaseBuilder builder = reflectionContext.SourceModel.IsInterface
                ? new InterfaceBuilder()
                : new ClassBuilder();
            return Result.Success((T)(object)builder);
        }

        return Result.Continue<T>();
    }
}
