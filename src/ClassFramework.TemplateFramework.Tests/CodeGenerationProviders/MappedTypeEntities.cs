namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MappedTypeEntities(IPipelineService pipelineService) : MappedCSharpClassBase(pipelineService)
{
    public override async Task<Result<IEnumerable<TypeBase>>> GetModel(CancellationToken cancellationToken) => await GetEntities([new ClassBuilder().WithName("MyClass").AddProperties(new PropertyBuilder().WithName("MyProperty").WithType(typeof(IMyMappedType))).Build()], "Test.Domain").ConfigureAwait(false);

    public override string Path => "Test.Domain";
}
