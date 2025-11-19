namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public class MultipleInterfacesOverrideEntities(ICommandService commandService) : MultipleInterfacesBase(commandService)
{
    public override string Path => "ClassFramework.Domain/Types";

    protected override bool EnableEntityInheritance => true;
    protected override bool CreateAsObservable => true;
    protected override Task<Result<TypeBase>> GetBaseClassAsync() => CreateBaseClassAsync(GetAbstractTypeAsync(), "ClassFramework.Domain");

    public override Task<Result<IEnumerable<TypeBase>>> GetModelAsync(CancellationToken token)
        => GetEntitiesAsync(GetOverrideTypesAsync(), "ClassFramework.Domain.Types");
}
