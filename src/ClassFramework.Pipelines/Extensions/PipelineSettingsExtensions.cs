namespace ClassFramework.Pipelines.Extensions;

public static class PipelineSettingsExtensions
{
    public static ArgumentValidationType AddValidationCode(this PipelineSettings instance)
    {
        if (instance.ValidateArguments == ArgumentValidationType.None)
        {
            // Do not validate arguments
            return ArgumentValidationType.None;
        }

        if (!instance.EnableInheritance)
        {
            // In case inheritance is enabled, then we want to add validation
            return instance.ValidateArguments;
        }

        if (instance.IsAbstract)
        {
            // Abstract class with base class
            return ArgumentValidationType.None;
        }

        if (instance.BaseClass is null)
        {
            // Abstract base class
            return ArgumentValidationType.None;
        }

        // In other situations, add it
        return instance.ValidateArguments;
    }
}
