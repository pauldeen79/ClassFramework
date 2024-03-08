namespace ClassFramework.Pipelines.Tests;

public abstract class TestBase : IDisposable
{
    protected IFixture Fixture { get; }

    protected TestBase()
    {
        Fixture = new Fixture().Customize(new AutoNSubstituteCustomization());

        Fixture.Customizations.Add(new BuilderOmitter());
    }

    protected ServiceProvider? Provider { get; set; }
    protected IServiceScope? Scope { get; set; }
    private IFormattableStringParser? _formattableStringParser;
    private bool disposedValue;

    private IFormattableStringParser FormattableStringParser
    {
        get
        {
            if (_formattableStringParser is null)
            {
                Provider = new ServiceCollection()
                    .AddParsers()
                    .AddPipelines()
                    .AddCsharpExpressionCreator()
                    .BuildServiceProvider();
                Scope = Provider.CreateScope();
                _formattableStringParser = Scope.ServiceProvider.GetRequiredService<IFormattableStringParser>();
            }

            return _formattableStringParser;
        }
    }

    protected IFormattableStringParser InitializeParser()
    {
        var parser = Fixture.Freeze<IFormattableStringParser>();
        var csharpExpressionCreator = Fixture.Freeze<ICsharpExpressionCreator>();
        csharpExpressionCreator.Create(Arg.Any<object?>()).Returns(x => x.ArgAt<object?>(0).ToStringWithNullCheck());
        
        // Pass through real IFormattableStringParser implementation, with all placeholder processors and stuff in our ClassFramework.Pipelines project.
        // One exception: If we supply "{Error}" as placeholder, then simply return an error with the error message "Kaboom".
        parser.Parse(Arg.Any<string>(), Arg.Any<IFormatProvider>(), Arg.Any<object?>())
              .Returns(x => x.ArgAt<string>(0) == "{Error}"
                ? Result.Error<string>("Kaboom")
                : FormattableStringParser.Parse(x.ArgAt<string>(0), x.ArgAt<IFormatProvider>(1), x.ArgAt<object?>(2))
                    .Transform(x => x.ErrorMessage == "Unknown placeholder in value: Error"
                        ? Result.Error<string>("Kaboom")
                        : x ));

        return parser;
    }

    protected static IConcreteType CreateModel(string baseClass = "", params MetadataBuilder[] propertyMetadataBuilders)
        => new ClassBuilder()
            .WithName("SomeClass")
            .WithNamespace("SomeNamespace")
            .WithBaseClass(baseClass)
            .AddProperties(new PropertyBuilder().WithName("Property1").WithType(typeof(int)).AddMetadata(propertyMetadataBuilders).AddAttributes(new AttributeBuilder().WithName("MyAttribute")))
            .AddProperties(new PropertyBuilder().WithName("Property2").WithType(typeof(string)).AddMetadata(propertyMetadataBuilders).AddAttributes(new AttributeBuilder().WithName("MyAttribute")))
            .AddProperties(new PropertyBuilder().WithName("Property3").WithType(typeof(List<int>)).AddMetadata(propertyMetadataBuilders).AddAttributes(new AttributeBuilder().WithName("MyAttribute")))
            .AddMetadata(new MetadataBuilder().WithName("MyMetadataName").WithValue("MyMetadataValue"))
            .BuildTyped();

    protected static Domain.Types.Interface CreateInterfaceModel(bool addProperties)
        => new InterfaceBuilder()
            .WithName("IMyClass")
            .WithNamespace("MyNamespace")
            .AddAttributes(new AttributeBuilder().WithName("MyAttribute"))
            .AddProperties(
                new[]
                {
                    new PropertyBuilder().WithName("Property1").WithType(typeof(string)).WithHasSetter(false),
                    new PropertyBuilder().WithName("Property2").WithTypeName(typeof(List<>).ReplaceGenericTypeName(typeof(string))).WithHasSetter(true)
                }.Where(_ => addProperties)
            )
            .AddMethods(new MethodBuilder().WithName("MyMethod"))
            .BuildTyped();

    protected static IConcreteType CreateGenericModel(bool addProperties)
        => new ClassBuilder()
            .WithName("MyClass")
            .WithNamespace("MyNamespace")
            .AddGenericTypeArguments("T")
            .AddGenericTypeArgumentConstraints("where T : class")
            .AddAttributes(new AttributeBuilder().WithName("MyAttribute"))
            .AddProperties(
                new[]
                {
                    new PropertyBuilder().WithName("Property1").WithType(typeof(string)).WithHasSetter(false),
                    new PropertyBuilder().WithName("Property2").WithTypeName(typeof(List<>).ReplaceGenericTypeName(typeof(string))).WithHasSetter(true)
                }.Where(_ => addProperties)
            )
            .BuildTyped();

    protected static IConcreteType CreateModelWithCustomTypeProperties()
        => new ClassBuilder()
            .WithName("MyClass")
            .WithNamespace("MySourceNamespace")
            .AddProperties(new PropertyBuilder().WithName("Property1").WithType(typeof(int)))
            .AddProperties(new PropertyBuilder().WithName("Property2").WithType(typeof(int)).WithIsNullable())
            .AddProperties(new PropertyBuilder().WithName("Property3").WithType(typeof(string)))
            .AddProperties(new PropertyBuilder().WithName("Property4").WithType(typeof(string)).WithIsNullable())
            .AddProperties(new PropertyBuilder().WithName("Property5").WithTypeName("MySourceNamespace.MyClass"))
            .AddProperties(new PropertyBuilder().WithName("Property6").WithTypeName("MySourceNamespace.MyClass").WithIsNullable())
            .AddProperties(new PropertyBuilder().WithName("Property7").WithTypeName(typeof(List<>).ReplaceGenericTypeName("MySourceNamespace.MyClass")))
            .AddProperties(new PropertyBuilder().WithName("Property8").WithTypeName(typeof(List<>).ReplaceGenericTypeName("MySourceNamespace.MyClass")).WithIsNullable())
            .BuildTyped();

    protected static Domain.Types.Interface CreateInterfaceModelWithCustomTypeProperties()
        => new InterfaceBuilder()
            .WithName("IMyClass")
            .WithNamespace("MySourceNamespace")
            .AddProperties(new PropertyBuilder().WithName("Property1").WithType(typeof(int)))
            .AddProperties(new PropertyBuilder().WithName("Property2").WithType(typeof(int)).WithIsNullable())
            .AddProperties(new PropertyBuilder().WithName("Property3").WithType(typeof(string)))
            .AddProperties(new PropertyBuilder().WithName("Property4").WithType(typeof(string)).WithIsNullable())
            .AddProperties(new PropertyBuilder().WithName("Property5").WithTypeName("MySourceNamespace.IMyClass"))
            .AddProperties(new PropertyBuilder().WithName("Property6").WithTypeName("MySourceNamespace.IMyClass").WithIsNullable())
            .AddProperties(new PropertyBuilder().WithName("Property7").WithTypeName(typeof(List<>).ReplaceGenericTypeName("MySourceNamespace.IMyClass")))
            .AddProperties(new PropertyBuilder().WithName("Property8").WithTypeName(typeof(List<>).ReplaceGenericTypeName("MySourceNamespace.IMyClass")).WithIsNullable())
            .BuildTyped();

    protected static IConcreteType CreateModelWithPropertyThatHasAReservedName(Type propertyType)
        => new ClassBuilder()
            .WithName("SomeClass")
            .WithNamespace("SomeNamespace")
            .AddProperties(new PropertyBuilder().WithName("Delegate").WithType(propertyType))
            .BuildTyped();

    protected static PipelineSettingsBuilder CreateSettingsForBuilder(
        bool enableBuilderInheritance = false,
        bool isAbstract = false,
        bool enableEntityInheritance = false,
        bool addNullChecks = false,
        bool enableNullableReferenceTypes = false,
        bool useExceptionThrowIfNull = false,
        bool copyAttributes = false,
        bool copyInterfaces = false,
        bool addCopyConstructor = false,
        bool setDefaultValues = true,
        bool inheritFromInterfaces = false,
        string newCollectionTypeName = "System.Collections.Generic.List",
        IEnumerable<NamespaceMappingBuilder>? namespaceMappings = null,
        IEnumerable<TypenameMappingBuilder>? typenameMappings = null,
        string setMethodNameFormatString = "With{Name}",
        string addMethodNameFormatString = "Add{Name}",
        string builderNamespaceFormatString = "{Namespace}.Builders",
        string builderNameFormatString = "{Class.Name}Builder",
        string buildMethodName = "Build",
        string buildTypedMethodName = "BuildTyped",
        string setDefaultValuesMethodName = "SetDefaultValues",
        ArgumentValidationType validateArguments = ArgumentValidationType.None,
        string? baseClassBuilderNameSpace = null,
        bool allowGenerationWithoutProperties = false,
        Class? baseClass = null,
        Func<IParentTypeContainer, IType, bool>? inheritanceComparisonDelegate = null,
        Predicate<Domain.Attribute>? copyAttributePredicate = null,
        Predicate<string>? copyInterfacePredicate = null)
        =>  CreateSettingsForEntity
            (
                enableEntityInheritance: enableEntityInheritance,
                addNullChecks: addNullChecks,
                useExceptionThrowIfNull: useExceptionThrowIfNull,
                enableNullableReferenceTypes: enableNullableReferenceTypes,
                validateArguments: validateArguments,
                allowGenerationWithoutProperties: allowGenerationWithoutProperties,
                copyAttributes: copyAttributes,
                copyInterfaces: copyInterfaces,
                copyAttributePredicate: copyAttributePredicate,
                copyInterfacePredicate: copyInterfacePredicate,
                inheritFromInterfaces: inheritFromInterfaces
            )
            .WithBuilderNewCollectionTypeName(newCollectionTypeName)
            .WithEnableNullableReferenceTypes(enableNullableReferenceTypes)
            .AddNamespaceMappings(namespaceMappings.DefaultWhenNull())
            .AddTypenameMappings(typenameMappings.DefaultWhenNull())
            .WithAddCopyConstructor(addCopyConstructor)
            .WithSetDefaultValuesInEntityConstructor(setDefaultValues)
            .WithEnableBuilderInheritance(enableBuilderInheritance)
            .WithIsAbstract(isAbstract)
            .WithBaseClass(baseClass?.ToBuilder())
            .WithBaseClassBuilderNameSpace(baseClassBuilderNameSpace ?? string.Empty)
            .WithInheritanceComparisonDelegate(inheritanceComparisonDelegate)
            .WithSetMethodNameFormatString(setMethodNameFormatString)
            .WithAddMethodNameFormatString(addMethodNameFormatString)
            .WithBuilderNamespaceFormatString(builderNamespaceFormatString)
            .WithBuilderNameFormatString(builderNameFormatString)
            .WithBuildMethodName(buildMethodName)
            .WithBuildTypedMethodName(buildTypedMethodName)
            .WithSetDefaultValuesMethodName(setDefaultValuesMethodName);

    protected static PipelineSettingsBuilder CreateSettingsForEntity(
        bool enableEntityInheritance = false,
        bool addNullChecks = false,
        bool useExceptionThrowIfNull = false,
        bool enableNullableReferenceTypes = false,
        bool copyAttributes = false,
        bool copyInterfaces = false,
        ArgumentValidationType validateArguments = ArgumentValidationType.None,
        bool addFullConstructor = true,
        bool addPublicParameterlessConstructor = false,
        bool allowGenerationWithoutProperties = false,
        bool isAbstract = false,
        Class? baseClass = null,
        string entityNamespaceFormatString = "{Namespace}",
        string entityNameFormatString = "{Class.Name}",
        string toBuilderFormatString = "ToBuilder",
        string toTypedBuilderFormatString = "ToTypedBuilder",
        string newCollectionTypeName = "System.Collections.Generic.IReadOnlyCollection",
        string collectionTypeName = "",
        bool addSetters = false,
        bool createRecord = false,
        bool addBackingFields = false,
        bool createAsObservable = false,
        bool inheritFromInterfaces = false,
        SubVisibility setterVisibility = SubVisibility.InheritFromParent,
        IEnumerable<NamespaceMappingBuilder>? namespaceMappings = null,
        IEnumerable<TypenameMappingBuilder>? typenameMappings = null,
        Predicate<Domain.Attribute>? copyAttributePredicate = null,
        Predicate<string>? copyInterfacePredicate = null)
        => new PipelineSettingsBuilder()
            .WithAllowGenerationWithoutProperties(allowGenerationWithoutProperties)
            .WithAddSetters(addSetters)
            .WithSetterVisibility(setterVisibility)
            .WithCreateRecord(createRecord)
            .WithAddBackingFields(addBackingFields)
            .WithCreateAsObservable(createAsObservable)
            .WithInheritFromInterfaces(inheritFromInterfaces)
            .WithAddNullChecks(addNullChecks)
            .WithUseExceptionThrowIfNull(useExceptionThrowIfNull)
            .WithEnableInheritance(enableEntityInheritance)
            .WithIsAbstract(isAbstract)
            .WithBaseClass(baseClass?.ToBuilder())
            .WithValidateArguments(validateArguments)
            .WithOriginalValidateArguments(validateArguments)
            .WithCollectionTypeName(collectionTypeName)
            .WithAddFullConstructor(addFullConstructor)
            .WithAddPublicParameterlessConstructor(addPublicParameterlessConstructor)
            .WithEntityNamespaceFormatString(entityNamespaceFormatString)
            .WithEntityNameFormatString(entityNameFormatString)
            .WithToBuilderFormatString(toBuilderFormatString)
            .WithToTypedBuilderFormatString(toTypedBuilderFormatString)
            .WithEntityNewCollectionTypeName(newCollectionTypeName)
            .WithEnableNullableReferenceTypes(enableNullableReferenceTypes)
            .AddNamespaceMappings(namespaceMappings.DefaultWhenNull())
            .AddTypenameMappings(typenameMappings.DefaultWhenNull())
            .WithCopyAttributes(copyAttributes)
            .WithCopyInterfaces(copyInterfaces)
            .WithCopyAttributePredicate(copyAttributePredicate)
            .WithCopyInterfacePredicate(copyInterfacePredicate);

    protected static PipelineSettingsBuilder CreateSettingsForOverrideEntity(
        bool enableEntityInheritance = false,
        bool addNullChecks = false,
        bool useExceptionThrowIfNull = false,
        bool enableNullableReferenceTypes = false,
        bool allowGenerationWithoutProperties = false,
        bool isAbstract = false,
        Class? baseClass = null,
        string entityNamespaceFormatString = "{Namespace}",
        string entityNameFormatString = "{Class.Name}",
        string newCollectionTypeName = "System.Collections.Generic.IReadOnlyCollection",
        bool createRecord = false,
        IEnumerable<NamespaceMappingBuilder>? namespaceMappings = null,
        IEnumerable<TypenameMappingBuilder>? typenameMappings = null)
        => new PipelineSettingsBuilder()
            .WithAllowGenerationWithoutProperties(allowGenerationWithoutProperties)
            .WithCreateRecord(createRecord)
            .WithAddNullChecks(addNullChecks)
            .WithUseExceptionThrowIfNull(useExceptionThrowIfNull)
            .WithEnableInheritance(enableEntityInheritance)
            .WithIsAbstract(isAbstract)
            .WithBaseClass(baseClass?.ToBuilder())
            .WithEntityNamespaceFormatString(entityNamespaceFormatString)
            .WithEntityNameFormatString(entityNameFormatString)
            .WithEntityNewCollectionTypeName(newCollectionTypeName)
            .WithEnableNullableReferenceTypes(enableNullableReferenceTypes)
            .AddNamespaceMappings(namespaceMappings.DefaultWhenNull())
            .AddTypenameMappings(typenameMappings.DefaultWhenNull());

    protected static PipelineSettingsBuilder CreateSettingsForReflection(
        bool copyAttributes = false,
        bool copyInterfaces = false,
        bool allowGenerationWithoutProperties = false,
        bool useBaseClassFromSourceModel = true,
        bool partial = true,
        bool createConstructors = true,
        bool enableEntityInheritance = false,
        bool isAbstract = false,
        string namespaceFormatString = "{Namespace}",
        string nameFormatString = "{Class.Name}",
        Class? baseClass = null,
        IEnumerable<NamespaceMappingBuilder>? namespaceMappings = null,
        IEnumerable<TypenameMappingBuilder>? typenameMappings = null,
        Predicate<Domain.Attribute>? copyAttributePredicate = null,
        Predicate<string>? copyInterfacePredicate = null)
        => new PipelineSettingsBuilder()
            .WithAllowGenerationWithoutProperties(allowGenerationWithoutProperties)
            .WithUseBaseClassFromSourceModel(useBaseClassFromSourceModel)
            .WithCreateAsPartial(partial)
            .WithCreateConstructors(createConstructors)
            .WithNamespaceFormatString(namespaceFormatString)
            .WithNameFormatString(nameFormatString)
            .AddNamespaceMappings(namespaceMappings.DefaultWhenNull())
            .AddTypenameMappings(typenameMappings.DefaultWhenNull())
            .WithEnableInheritance(enableEntityInheritance)
            .WithIsAbstract(isAbstract)
            .WithBaseClass(baseClass?.ToBuilder())
            .WithCopyAttributes(copyAttributes)
            .WithCopyInterfaces(copyInterfaces)
            .WithCopyAttributePredicate(copyAttributePredicate)
            .WithCopyInterfacePredicate(copyInterfacePredicate);

    protected static PipelineSettingsBuilder CreateSettingsForInterface(
        bool addSetters = false,
        bool copyAttributes = false,
        bool copyInterfaces = false,
        bool copyMethods = false,
        bool allowGenerationWithoutProperties = false,
        bool enableEntityInheritance = false,
        bool isAbstract = false,
        string namespaceFormatString = "{Namespace}",
        string nameFormatString = "{Class.Name}",
        string newCollectionTypeName = "System.Collections.Generic.IReadOnlyCollection",
        Class? baseClass = null,
        IEnumerable<NamespaceMappingBuilder>? namespaceMappings = null,
        IEnumerable<TypenameMappingBuilder>? typenameMappings = null,
        Predicate<Domain.Attribute>? copyAttributePredicate = null,
        Predicate<string>? copyInterfacePredicate = null,
        Func<IType, Method, bool>? copyMethodPredicate = null)
        => new PipelineSettingsBuilder()
            .WithAddSetters(addSetters)
            .WithAllowGenerationWithoutProperties(allowGenerationWithoutProperties)
            .WithNamespaceFormatString(namespaceFormatString)
            .WithNameFormatString(nameFormatString)
            .WithEnableInheritance(enableEntityInheritance)
            .WithIsAbstract(isAbstract)
            .WithBaseClass(baseClass?.ToBuilder())
            .WithEntityNewCollectionTypeName(newCollectionTypeName)
            .AddNamespaceMappings(namespaceMappings.DefaultWhenNull())
            .AddTypenameMappings(typenameMappings.DefaultWhenNull())
            .WithCopyAttributes(copyAttributes)
            .WithCopyInterfaces(copyInterfaces)
            .WithCopyMethods(copyMethods)
            .WithCopyAttributePredicate(copyAttributePredicate)
            .WithCopyInterfacePredicate(copyInterfacePredicate)
            .WithCopyMethodPredicate(copyMethodPredicate);

    protected static IEnumerable<NamespaceMappingBuilder> CreateNamespaceMappings(string sourceNamespace = "MySourceNamespace")
        =>
        [
            new NamespaceMappingBuilder().WithSourceNamespace(sourceNamespace).WithTargetNamespace("MyNamespace")
                .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderNamespace).WithValue("MyNamespace.Builders"))
                .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderName).WithValue("{TypeName.ClassName}Builder"))
                .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomEntityNamespace).WithValue("MyNamespace"))
                .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderSourceExpression).WithValue("[Name][NullableSuffix].ToBuilder()"))
                .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderMethodParameterExpression).WithValue("[Name][NullableSuffix].Build()"))
        ];

    protected static IEnumerable<TypenameMappingBuilder> CreateTypenameMappings()
        =>
        [
            new TypenameMappingBuilder().WithSourceTypeName(typeof(List<>).WithoutGenerics()).WithTargetTypeName(typeof(List<>).WithoutGenerics()).AddMetadata(MetadataNames.CustomCollectionInitialization, "new [Type]<[Generics]>([Expression])"), //"[Expression].ToList()"
            new TypenameMappingBuilder().WithSourceTypeName(typeof(IList<>).WithoutGenerics()).WithTargetTypeName(typeof(IList<>).WithoutGenerics()).AddMetadata(MetadataNames.CustomCollectionInitialization, "[Expression].ToList()"),
        ];

    protected static TypenameMappingBuilder[] CreateExpressionFrameworkTypenameMappings()
        =>
        [
            new TypenameMappingBuilder()
                .WithSourceTypeName("ExpressionFramework.Domain.Evaluatables.ComposedEvaluatable")
                .WithTargetTypeName("ExpressionFramework.Domain.Evaluatables.ComposedEvaluatable")
                .AddMetadata
                (
                    new MetadataBuilder().WithValue("ExpressionFramework.Domain.Builders.Evaluatables").WithName(MetadataNames.CustomBuilderNamespace),
                    new MetadataBuilder().WithValue("{TypeName.ClassName}Builder").WithName(MetadataNames.CustomBuilderName),
                    new MetadataBuilder().WithValue("new ExpressionFramework.Domain.Builders.Evaluatables.ComposedEvaluatableBuilder(source.[Name])").WithName(MetadataNames.CustomBuilderConstructorInitializeExpression),
                    new MetadataBuilder().WithValue(new Literal("new ExpressionFramework.Domain.Builders.Evaluatables.ComposedEvaluatableBuilder()", null)).WithName(MetadataNames.CustomBuilderDefaultValue),
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].BuildTyped()").WithName(MetadataNames.CustomBuilderMethodParameterExpression)
                ),
            new TypenameMappingBuilder()
                .WithSourceTypeName("ExpressionFramework.Domain.Expression")
                .WithTargetTypeName("ExpressionFramework.Domain.Expression")
                .AddMetadata
                (
                    new MetadataBuilder().WithValue("ExpressionFramework.Domain.Builders").WithName(MetadataNames.CustomBuilderNamespace),
                    new MetadataBuilder().WithValue("{TypeName.ClassName}Builder").WithName(MetadataNames.CustomBuilderName),
                    new MetadataBuilder().WithValue("ExpressionFramework.Domain.Builders.ExpressionBuilderFactory.Create(source.[Name])").WithName(MetadataNames.CustomBuilderConstructorInitializeExpression),
                    new MetadataBuilder().WithValue(new Literal("default(ExpressionFramework.Domain.Builders.ExpressionBuilder)!", null)).WithName(MetadataNames.CustomBuilderDefaultValue),
                    new MetadataBuilder().WithValue($"[Name][NullableSuffix].Build()").WithName(MetadataNames.CustomBuilderMethodParameterExpression)
                ),
        ];

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Scope?.Dispose();
                Provider?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public abstract class TestBase<T> : TestBase
{
    protected T CreateSut() => Fixture.Create<T>();
}

internal sealed class BuilderOmitter : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var propInfo = request as System.Reflection.PropertyInfo;
        if (propInfo is not null && propInfo.DeclaringType?.Name.Contains("Builder", StringComparison.Ordinal) == true)
        {
            return new OmitSpecimen();
        }

        return new NoSpecimen();
    }
}
