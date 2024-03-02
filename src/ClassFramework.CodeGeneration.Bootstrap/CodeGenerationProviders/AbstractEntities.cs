namespace ClassFramework.CodeGeneration.Bootstrap.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public class AbstractEntities : ClassFrameworkCSharpClassBase
{
    public override string Path => Constants.Paths.Domain;

    protected override bool EnableEntityInheritance => true;
    protected override bool EnableBuilderInhericance => true;
    protected override bool IsAbstract => true;

    protected override ModelFramework.Objects.Settings.ArgumentValidationType ValidateArgumentsInConstructor => ModelFramework.Objects.Settings.ArgumentValidationType.None; // not needed for abstract entities, because each derived class will do its own validation
    protected override bool AddNullChecks => false; // not needed for abstract entities, because each derived class will do its own validation

    public override object CreateModel()
        => GetImmutableClasses(GetAbstractModels(), Constants.Namespaces.Domain)
            .OfType<ModelFramework.Objects.Contracts.IClass>()
            .Select(x => new ModelFramework.Objects.Builders.ClassBuilder(x)
                .AddMethods(new ModelFramework.Objects.Builders.ClassMethodBuilder()
                    .WithName("ToBuilder")
                    .WithAbstract()
                    .WithTypeName($"{Constants.Namespaces.DomainBuilders}.{x.Name}Builder")
                )
                .BuildTyped()
            )
            .ToArray();
}
