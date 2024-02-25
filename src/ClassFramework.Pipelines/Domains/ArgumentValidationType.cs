namespace ClassFramework.Pipelines.Domains;

public enum ArgumentValidationType
{
    /// <summary>
    /// Do not validate arguments
    /// </summary>
    None,
    /// <summary>
    /// Validate arguments in builder and domain entity, re-using the domain entity validation code
    /// </summary>
    Shared,
    /// <summary>
    /// Validate arguments in domain entity only. When building the entity from the builder, the entity will validate itself.
    /// </summary>
    DomainOnly
}
