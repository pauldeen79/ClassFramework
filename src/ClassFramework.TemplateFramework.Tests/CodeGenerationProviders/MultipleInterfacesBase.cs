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

    protected static Task<Result<IEnumerable<TypeBase>>> GetAbstractionsTypes()
        => Task.FromResult(Result.Success<IEnumerable<TypeBase>>(
            [
                new InterfaceBuilder().WithName("IDefaultValueContainer").WithNamespace("ClassFramework.Domain.Abstractions").AddProperties(new PropertyBuilder().WithName("DefaultValue").WithType(typeof(object)).WithIsNullable().WithHasSetter(false)).Build(),
                new InterfaceBuilder().WithName("INameContainer").WithNamespace("ClassFramework.Domain.Abstractions").AddProperties(new PropertyBuilder().WithName("Name").WithType(typeof(string)).WithHasSetter(false)).Build(),
                new InterfaceBuilder().WithName("IType").WithNamespace("ClassFramework.Domain.Abstractions").AddProperties(new PropertyBuilder().WithName("Namespace").WithType(typeof(string)).WithHasSetter(false)).AddInterfaces("ClassFramework.Domain.Abstractions.INameContainer", "ClassFramework.Domain.Abstractions.IDefaultValueContainer").Build()
            ]));

    protected static Task<Result<IEnumerable<TypeBase>>> GetAbstractTypes()
        => Task.FromResult(Result.Success<IEnumerable<TypeBase>>(
            [
                new InterfaceBuilder().WithName("ITypeBase").WithNamespace("ClassFramework.Domain").AddInterfaces("ClassFramework.Domain.Abstractions.IType", "ClassFramework.Domain.Abstractions.INameContainer", "ClassFramework.Domain.Abstractions.IDefaultValueContainer").Build(),
            ]));
}
