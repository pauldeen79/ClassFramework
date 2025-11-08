namespace ClassFramework.Pipelines.PipelineResponseGeneratorComponents;

public class ReflectionContextPipelineResponseGeneratorComponent : IPipelineResponseGeneratorComponent
{
    public Result<T> Generate<T>(object command)
    {
        if (command is Reflection.ReflectionContext reflectionContext && typeof(TypeBaseBuilder).IsAssignableFrom(typeof(T)))
        {
            return Result.Success<T>((T)Convert.ChangeType(reflectionContext.SourceModel.IsInterface
                ? new InterfaceBuilder()
                : new ClassBuilder(), typeof(T)));
        }

        return Result.Continue<T>();
    }
}
