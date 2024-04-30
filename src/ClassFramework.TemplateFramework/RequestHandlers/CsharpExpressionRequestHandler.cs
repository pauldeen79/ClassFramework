namespace ClassFramework.TemplateFramework.RequestHandlers;

public class CsharpExpressionRequestHandler : IRequestHandler<CsharpExpressionRequest, string>
{
    private readonly ICsharpExpressionDumper _csharpExpressionDumper;

    public CsharpExpressionRequestHandler(ICsharpExpressionDumper csharpExpressionDumper)
    {
        _csharpExpressionDumper = csharpExpressionDumper.IsNotNull(nameof(csharpExpressionDumper));
    }

    public Task<string> Handle(CsharpExpressionRequest request, CancellationToken cancellationToken)
        => Task.FromResult(_csharpExpressionDumper.Dump(request.IsNotNull(nameof(request)).Expression));
}
