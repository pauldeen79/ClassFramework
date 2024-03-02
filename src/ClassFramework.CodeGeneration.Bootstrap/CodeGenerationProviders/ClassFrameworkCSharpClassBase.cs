namespace ClassFramework.CodeGeneration.CodeGenerationProviders;

[ExcludeFromCodeCoverage]
public abstract class ClassFrameworkCSharpClassBase : CSharpClassBase
{
    public override bool RecurseOnDeleteGeneratedFiles => false;
    public override string DefaultFileName => string.Empty; // not used because we're using multiple files, but it's abstract so we need to fill it

    protected override bool CreateCodeGenerationHeader => true;
    protected override bool EnableNullableContext => true;
    protected override Type RecordCollectionType => typeof(IReadOnlyCollection<>);
    protected override Type RecordConcreteCollectionType => typeof(List<>);
    protected override string FileNameSuffix => Constants.TemplateGenerated;
    protected override string ProjectName => Constants.ProjectName;
    protected override bool UseLazyInitialization => false; // we don't want lazy stuff, just getters and setters
    protected override bool AddBackingFieldsForCollectionProperties => false; // we just want static stuff - else you need to choose builders or models instead of entities
    protected override bool AddPrivateSetters => false; // we just want static stuff - else you need to choose builders or models instead of entities
    protected override ModelFramework.Objects.Settings.ArgumentValidationType ValidateArgumentsInConstructor => ModelFramework.Objects.Settings.ArgumentValidationType.DomainOnly; // note that for now, we don't have any custom validation, so we can simply use domain validation and copy the attributes to the builder
    protected override bool ConvertStringToStringBuilderOnBuilders => false; // we don't want string builders, just strings
    protected override bool AddNullChecks => true;
    protected override bool CopyAttributes => true; // Copy validation attributes to builders (note: set to false in case of shared validation!)

    protected override void Visit<TBuilder, TEntity>(ModelFramework.Objects.Builders.TypeBaseBuilder<TBuilder, TEntity> typeBaseBuilder)
    {
        Guard.IsNotNull(typeBaseBuilder);

        foreach (var property in typeBaseBuilder.Properties)
        {
            if (property.TypeName == "ClassFramework.Domain.Domains.ArgumentValidationType")
            {
                property.TypeName = "ClassFramework.Pipelines.Domains.ArgumentValidationType";
            }
            else if (property.TypeName == "System.Nullable<ClassFramework.Domain.Domains.ArgumentValidationType>")
            {
                property.TypeName = "System.Nullable<ClassFramework.Pipelines.Domains.ArgumentValidationType>";
            }
        }

        if (typeBaseBuilder.Name.EndsWith(BuilderName))
        {
            FixBuilder(typeBaseBuilder);
        }
        else
        {
            FixEntity(typeBaseBuilder);
        }
    }

    protected override Dictionary<string, string> GetModelMappings()
    {
        var result = base.GetModelMappings();

        result.Add($"{CodeGenerationRootNamespace}.Models.Pipelines.I", $"ClassFramework.Pipelines.");

        return result;
    }

    protected override IEnumerable<KeyValuePair<string, string>> GetCustomBuilderNamespaceMapping()
    {
        yield return new KeyValuePair<string, string>("ClassFramework.Pipelines", "ClassFramework.Pipelines.Builders");
    }

    protected ModelFramework.Objects.Contracts.ITypeBase[] GetPipelinesModels()
        => MapCodeGenerationModelsToDomain(
            GetType().Assembly.GetTypes()
                .Where(x => x.IsInterface && x.Namespace == $"{CodeGenerationRootNamespace}.Models.Pipelines" && !GetCustomBuilderTypes().Contains(x.GetEntityClassName())));

    protected ModelFramework.Objects.Contracts.ITypeBase[] GetTemplateFrameworkModels()
        => MapCodeGenerationModelsToDomain(
            GetType().Assembly.GetTypes()
                .Where(x => x.IsInterface && x.Namespace == $"{CodeGenerationRootNamespace}.Models.TemplateFramework" && !GetCustomBuilderTypes().Contains(x.GetEntityClassName())));

    protected ModelFramework.Objects.Contracts.IClass FixOverrideEntity(ModelFramework.Objects.Contracts.IClass cls, string entityName, string buildersNamespace)
    {
        cls = cls.IsNotNull(nameof(cls));

        return new ModelFramework.Objects.Builders.ClassBuilder(cls)
            .AddMethods
            (
                new ModelFramework.Objects.Builders.ClassMethodBuilder()
                    .WithName("ToBuilder")
                    .WithTypeName($"{Constants.Namespaces.DomainBuilders}.{entityName}BaseBuilder")
                    .WithOverride()
                    .AddLiteralCodeStatements("return ToTypedBuilder();"),
                new ModelFramework.Objects.Builders.ClassMethodBuilder()
                    .WithName("ToTypedBuilder")
                    .WithTypeName($"{buildersNamespace}.{cls.Name.ReplaceSuffix("Base", string.Empty, StringComparison.Ordinal)}Builder")
                    .WithOverride(!cls.Name.EndsWith("Base") && ValidateArgumentsInConstructor == ModelFramework.Objects.Settings.ArgumentValidationType.Shared)
                    .WithVirtual(cls.Name.EndsWith("Base"))
                    .AddLiteralCodeStatements(cls.Name.EndsWith("Base")
                        ? $"throw new {typeof(NotSupportedException).FullName}(\"You can't convert a base class to builder\");"
                        : $"return new {buildersNamespace}.{cls.Name}Builder(this);")
            ).BuildTyped();
    }

    protected bool IsInterfacedMethod(string methodName, ModelFramework.Objects.Builders.ClassBuilder classBuilder)
        => classBuilder.IsNotNull(nameof(classBuilder)).Properties
            .Where(x => x.ParentTypeFullName.StartsWith("ClassFramework.CodeGeneration.Models.Abstractions.", StringComparison.Ordinal))
            .Select(x => x.Name)
            .Any(x => methodName == $"{SetMethodNameFormatString.Replace("{0}", string.Empty, StringComparison.Ordinal)}{x}" || methodName == $"{AddMethodNameFormatString.Replace("{0}", string.Empty, StringComparison.Ordinal)}{x}");

    private void FixBuilder<TBuilder, TEntity>(ModelFramework.Objects.Builders.TypeBaseBuilder<TBuilder, TEntity> typeBaseBuilder)
        where TBuilder : ModelFramework.Objects.Builders.TypeBaseBuilder<TBuilder, TEntity>
        where TEntity : ModelFramework.Objects.Contracts.ITypeBase
    {
        if (typeBaseBuilder is ModelFramework.Objects.Builders.ClassBuilder classBuilder)
        {
            classBuilder.AddMethods(new ModelFramework.Objects.Builders.ClassMethodBuilder().WithName("SetDefaultValues").WithPartial().WithVisibility(ModelFramework.Objects.Contracts.Visibility.Private));
            classBuilder.Constructors.First(x => x.Parameters.Count == 0).CodeStatements.Add(new ModelFramework.Objects.CodeStatements.Builders.LiteralCodeStatementBuilder("SetDefaultValues();"));
        }

        var sourceModel = Array.Find(GetType().Assembly.GetTypes(), x => x.Name == $"I{typeBaseBuilder.Name.ReplaceSuffix(BuilderName, string.Empty, StringComparison.Ordinal)}");
        if (sourceModel is not null)
        {
            var interfaces = sourceModel.GetInterfaces();
            foreach (var i in interfaces.Where(x => x.FullName is not null && x.FullName.Contains($"{CodeGenerationRootNamespace}.Models.Abstractions.", StringComparison.Ordinal)))
            {
                typeBaseBuilder.AddInterfaces(i.FullName!.Replace($"{CodeGenerationRootNamespace}.Models.Abstractions.", $"{RootNamespace}.{BuildersName}.Abstractions.", StringComparison.Ordinal) + BuilderName);
            }
        }
    }

    private void FixEntity<TBuilder, TEntity>(ModelFramework.Objects.Builders.TypeBaseBuilder<TBuilder, TEntity> typeBaseBuilder)
        where TBuilder : ModelFramework.Objects.Builders.TypeBaseBuilder<TBuilder, TEntity>
        where TEntity : ModelFramework.Objects.Contracts.ITypeBase
    {
        ConvertBuilderFactoriesToToBuilderMethod(typeBaseBuilder);
        AddAbstractionsInterfaces(typeBaseBuilder);
    }

    private void AddAbstractionsInterfaces<TBuilder, TEntity>(ModelFramework.Objects.Builders.TypeBaseBuilder<TBuilder, TEntity> typeBaseBuilder)
        where TBuilder : ModelFramework.Objects.Builders.TypeBaseBuilder<TBuilder, TEntity>
        where TEntity : ModelFramework.Objects.Contracts.ITypeBase
    {
        var sourceModel = Array.Find(GetType().Assembly.GetTypes(), x => x.Name == $"I{typeBaseBuilder.Name}");
        if (sourceModel is null)
        {
            return;
        }

        var interfaces = sourceModel.GetInterfaces();
        foreach (var i in interfaces.Where(x => x.FullName is not null && x.FullName.Contains($"{CodeGenerationRootNamespace}.Models.Abstractions.", StringComparison.Ordinal)))
        {
            typeBaseBuilder.AddInterfaces(i.FullName!
                .Replace($"{CodeGenerationRootNamespace}.Models.Abstractions.", $"{RootNamespace}.Abstractions.", StringComparison.Ordinal)
                .Replace($"{CodeGenerationRootNamespace}.Models.", $"{RootNamespace}.", StringComparison.Ordinal));
        }
    }

    private void ConvertBuilderFactoriesToToBuilderMethod<TBuilder, TEntity>(ModelFramework.Objects.Builders.TypeBaseBuilder<TBuilder, TEntity> typeBaseBuilder)
        where TBuilder : ModelFramework.Objects.Builders.TypeBaseBuilder<TBuilder, TEntity>
        where TEntity : ModelFramework.Objects.Contracts.ITypeBase
    {
        foreach (var property in typeBaseBuilder.Properties)
        {
            if (!string.IsNullOrEmpty(GetEntityClassName(property.TypeName)))
            {
                var value = property.IsNullable
                    ? "if (source.{0} is not null) _{1} = source.{0}.ToBuilder()"
                    : "_{1} = source.{0}.ToBuilder()";

                property.Metadata.Replace(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, value);
            }

            if (!string.IsNullOrEmpty(GetEntityClassName(property.TypeName.GetGenericArguments())))
            {
                var value = property.IsNullable
                    ? "if (source.{0} is not null) _{1}.AddRange(source.{0}.Select(x => x.ToBuilder()))"
                    : "_{1}.AddRange(source.{0}.Select(x => x.ToBuilder()))";

                property.Metadata.Replace(ModelFramework.Objects.MetadataNames.CustomBuilderConstructorInitializeExpression, value);
            }
        }
    }
}
