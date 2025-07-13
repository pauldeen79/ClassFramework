namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class MultipleInterfacesBase(IPipelineService pipelineService) : CsharpClassGeneratorPipelineCodeGenerationProviderBase(pipelineService)
{
    public override bool RecurseOnDeleteGeneratedFiles => false;
    public override string LastGeneratedFilesFilename => string.Empty;
    public override Encoding Encoding => Encoding.UTF8;

    protected override Type EntityCollectionType => typeof(IReadOnlyCollection<>);
    protected override Type EntityConcreteCollectionType => typeof(ReadOnlyCollection<>);
    protected override Type BuilderCollectionType => typeof(ObservableCollection<>);

    protected override string ProjectName => "ClassFramework";
    protected override string CoreNamespace => "ClassFramework.Domain";
    protected override bool CopyAttributes => true;
    protected override bool CopyInterfaces => true;
    protected override bool EnableGlobalUsings => true;
    protected override bool AddImplicitOperatorOnBuilder => true;
    protected override bool CreateCodeGenerationHeader => false;
    protected override bool UseBuilderAbstractionsTypeConversion => true;

    protected static Task<Result<IEnumerable<TypeBase>>> GetAbstractionsTypesAsync()
        => Task.FromResult(Result.Success<IEnumerable<TypeBase>>(
            [
                new InterfaceBuilder().WithName("IDefaultValueContainer").WithNamespace("ClassFramework.Domain.Abstractions").AddProperties(new PropertyBuilder().WithName("DefaultValue").WithType(typeof(object)).WithIsNullable().WithHasSetter(false).WithParentTypeFullName("ClassFramework.Domain.Abstractions.IDefaultValueContainer")).Build(),
                new InterfaceBuilder().WithName("INameContainer").WithNamespace("ClassFramework.Domain.Abstractions").AddProperties(new PropertyBuilder().WithName("Name").WithType(typeof(string)).WithHasSetter(false).WithParentTypeFullName("ClassFramework.Domain.Abstractions.INameContainer")).Build(),
                new InterfaceBuilder().WithName("IType").WithNamespace("ClassFramework.Domain.Abstractions").AddProperties(new PropertyBuilder().WithName("Namespace").WithType(typeof(string)).WithHasSetter(false).WithParentTypeFullName("ClassFramework.Domain.Abstractions.IType")).AddInterfaces("ClassFramework.Domain.Abstractions.INameContainer", "ClassFramework.Domain.Abstractions.IDefaultValueContainer").Build()
            ]));

    protected static Task<Result<IEnumerable<TypeBase>>> GetAbstractTypesAsync()
        => Task.FromResult(Result.Success<IEnumerable<TypeBase>>([ GetAbstractTypeBase() ]));

    protected static Task<Result<TypeBase>> GetAbstractTypeAsync()
        => Task.FromResult(Result.Success(GetAbstractTypeBase()));

    private static TypeBase GetAbstractTypeBase()
        => new InterfaceBuilder()
            .WithName("ITypeBase")
            .WithNamespace("ClassFramework.Domain")
            .AddInterfaces("ClassFramework.Domain.Abstractions.IType", "ClassFramework.Domain.Abstractions.INameContainer", "ClassFramework.Domain.Abstractions.IDefaultValueContainer")
            .Build();

    protected async Task<Result<TypeBase>> CreateBaseClassAsync(Task<Result<TypeBase>> baseClassTypeResult, string @namespace)
    {
        Guard.IsNotNull(baseClassTypeResult);
        Guard.IsNotNull(@namespace);

        return await ProcessBaseClassResultAsync(baseClassTypeResult, GenerateBaseClass(@namespace)).ConfigureAwait(false);
    }

    protected static Task<Result<IEnumerable<TypeBase>>> GetOverrideTypesAsync()
        => Task.FromResult(Result.Success<IEnumerable<TypeBase>>(
            [
                new ClassBuilder().WithNamespace("ClassFramework.Domain.Types").WithName("Class").AddInterfaces("ClassFramework.Domain.ITypeBase").Build()
            ]));

    // this quirk is only needed because I build types using TypeBase derived entities instead of reflection
    protected override IEnumerable<TypenameMappingBuilder> GetAdditionalTypenameMappings()
    {
        foreach (var item in CreateBuilderAbstractionTypeConversionTypenameMapping("DefaultValueContainer", string.Empty))
        {
            yield return item;
        }

        foreach (var item in CreateBuilderAbstractionTypeConversionTypenameMapping("NameContainer", string.Empty))
        {
            yield return item;
        }

        foreach (var item in CreateBuilderAbstractionTypeConversionTypenameMapping("Type", string.Empty))
        {
            yield return item;
        }
    }
}
