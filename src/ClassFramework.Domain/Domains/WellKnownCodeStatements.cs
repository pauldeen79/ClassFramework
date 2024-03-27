namespace ClassFramework.Domain.Domains;

public static class WellKnownCodeStatements
{
    public static readonly string ThrowNotImplementedException = $"throw new {typeof(NotImplementedException).FullName}();";
}
