namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MappedTypeBuilders(ICommandService commandService) : MappedCSharpClassBase(commandService)
{
    public override async Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token) => await GetBuildersAsync(Task.FromResult(Result.Success<IEnumerable<TypeBase>>([new ClassBuilder().WithName("MyClass").AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(IMyMappedType))).Build()])), "Test.Domain.Builders", "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders";
}
