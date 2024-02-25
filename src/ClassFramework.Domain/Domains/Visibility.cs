namespace ClassFramework.Domain.Domains;

public enum Visibility
{
    /// <summary>
    /// Visible to everyone
    /// </summary>
    Public,
    /// <summary>
    /// Visible to the class itself, and all other classes within the same assembly
    /// </summary>
    Internal,
    /// <summary>
    /// Visible to the class itself only
    /// </summary>
    Private
}
