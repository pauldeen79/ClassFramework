namespace ClassFramework.CsharpExpressionCreator.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCsharpExpressionCreator(this IServiceCollection services)
        => services
        .AddTransient<ICsharpExpressionCreator, DefaultCsharpExpressionCreator>()
        .AddCsharpExpressionDumper();
}
