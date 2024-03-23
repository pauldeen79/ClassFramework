namespace ClassFramework.Pipelines.Domains;

public enum ArgumentValidationType
{
    /// <summary>
    /// Do not validate arguments
    /// </summary>
    None,
    /// <summary>
    /// Validate arguments in entity using standard IValidatableObject validation. When building the entity from the builder, the entity will validate itself.
    /// </summary>
    IValidatableObject
}
