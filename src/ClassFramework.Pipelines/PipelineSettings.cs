namespace ClassFramework.Pipelines;

public partial record PipelineSettings
{
    public ArgumentValidationType AddValidationCode
    {
        get
        {
            if (ValidateArguments == ArgumentValidationType.None)
            {
                // Do not validate arguments
                return ArgumentValidationType.None;
            }

            if (!EnableInheritance)
            {
                // In case inheritance is enabled, then we want to add validation
                return ValidateArguments;
            }

            if (IsAbstract)
            {
                // Abstract class with base class
                return ArgumentValidationType.None;
            }

            if (BaseClass is null)
            {
                // Abstract base class
                return ArgumentValidationType.None;
            }

            // In other situations, add it
            return ValidateArguments;
        }
    }
}
