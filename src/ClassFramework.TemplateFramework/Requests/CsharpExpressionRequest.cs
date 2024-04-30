namespace ClassFramework.TemplateFramework.Requests;

public class CsharpExpressionRequest : IRequest<string>
{
    public CsharpExpressionRequest(object? expression)
    {
        Expression = expression;
    }

    public object? Expression { get; }
}
