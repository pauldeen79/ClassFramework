namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MappedTypeBuilders(IPipelineService pipelineService) : MappedCSharpClassBase(pipelineService)
{
    public override async Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => await GetBuilders(Result.Success<IEnumerable<TypeBase>>([new ClassBuilder().WithName("MyClass").AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(IMyMappedType))).Build()]), "Test.Domain.Builders", "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain/Builders";
}
