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
                    .AddExpressionEvaluator()
                    .AddClassFrameworkPipelines()
                    .AddCsharpExpressionDumper()
                    .BuildServiceProvider();
                Scope = Provider.CreateScope();
                _formattableStringParser = Scope.ServiceProvider.GetRequiredService<IFormattableStringParser>();
            }

            return _formattableStringParser;
        }
    }

    protected IFormattableStringParser InitializeParser(bool forceError = false)
    {
        var parser = Fixture.Freeze<IFormattableStringParser>();
        var csharpExpressionDumper = Fixture.Freeze<ICsharpExpressionDumper>();
        csharpExpressionDumper.Dump(Arg.Any<object?>(), Arg.Any<Type?>()).Returns(x => x.ArgAt<object?>(0).ToStringWithNullCheck());

        // Pass through real IFormattableStringParser implementation, with all placeholder processors and stuff in our ClassFramework.Pipelines project.
        // One exception: If we supply "{Error}" as placeholder, then simply return an error with the error message "Kaboom".
        parser.Parse(Arg.Any<string>(), Arg.Any<FormattableStringParserSettings>(), Arg.Any<object?>())
              .Returns(x => forceError || x.ArgAt<string>(0) == "{Error}"
                ? Result.Error<GenericFormattableString>("Kaboom")
                : FormattableStringParser.Parse(x.ArgAt<string>(0), x.ArgAt<FormattableStringParserSettings>(1), x.ArgAt<object?>(2))
                    .Transform(x => x.ErrorMessage == "Unknown placeholder in value: Error"
                        ? Result.Error<GenericFormattableString>("Kaboom")
                        : x));

        return parser;
    }

    protected static Class CreateClass(string baseClass = "")
        => new ClassBuilder()
            .WithName("SomeClass")
            .WithNamespace("SomeNamespace")
            .WithBaseClass(baseClass)
            .AddProperties(new PropertyBuilder().WithName("Property1").WithType(typeof(int)).AddAttributes(new AttributeBuilder().WithName("MyAttribute")))
            .AddProperties(new PropertyBuilder().WithName("Property2").WithType(typeof(string)).AddAttributes(new AttributeBuilder().WithName("MyAttribute")))
            .AddProperties(new PropertyBuilder().WithName("Property3").WithType(typeof(List<int>)).AddAttributes(new AttributeBuilder().WithName("MyAttribute")))
            .BuildTyped();

    protected static Domain.Types.Interface CreateInterface(bool addProperties)
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

    protected static Class CreateGenericClass(bool addProperties)
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

    protected static Class CreateClassWithCustomTypeProperties(IEquatableItemType itemType = IEquatableItemType.Properties)
    {
        var builder = new ClassBuilder()
            .WithName("MyClass")
            .WithNamespace("MySourceNamespace");

        if (itemType == IEquatableItemType.Properties)
        {
            builder
                .AddProperties(
                    new PropertyBuilder().WithName("Property1").WithType(typeof(int)),
                    new PropertyBuilder().WithName("Property2").WithType(typeof(int)).WithIsNullable(),
                    new PropertyBuilder().WithName("Property3").WithType(typeof(string)),
                    new PropertyBuilder().WithName("Property4").WithType(typeof(string)).WithIsNullable(),
                    new PropertyBuilder().WithName("Property5").WithTypeName("MySourceNamespace.MyClass"),
                    new PropertyBuilder().WithName("Property6").WithTypeName("MySourceNamespace.MyClass").WithIsNullable(),
                    new PropertyBuilder().WithName("Property7").WithTypeName(typeof(List<>).ReplaceGenericTypeName("MySourceNamespace.MyClass")),
                    new PropertyBuilder().WithName("Property8").WithTypeName(typeof(List<>).ReplaceGenericTypeName("MySourceNamespace.MyClass")).WithIsNullable());
        }

        if (itemType == IEquatableItemType.Fields)
        {
            builder
                .AddFields(
                    new FieldBuilder().WithName("_field1").WithType(typeof(int)),
                    new FieldBuilder().WithName("_field2").WithType(typeof(int)).WithIsNullable(),
                    new FieldBuilder().WithName("_field3").WithType(typeof(string)),
                    new FieldBuilder().WithName("_field4").WithType(typeof(string)).WithIsNullable(),
                    new FieldBuilder().WithName("_field5").WithTypeName("MySourceNamespace.MyClass"),
                    new FieldBuilder().WithName("_field6").WithTypeName("MySourceNamespace.MyClass").WithIsNullable(),
                    new FieldBuilder().WithName("_field7").WithTypeName(typeof(List<>).ReplaceGenericTypeName("MySourceNamespace.MyClass")),
                    new FieldBuilder().WithName("_field8").WithTypeName(typeof(List<>).ReplaceGenericTypeName("MySourceNamespace.MyClass")).WithIsNullable());
        }

        return builder.BuildTyped();
    }

    protected static Domain.Types.Interface CreateInterfaceWithCustomTypeProperties()
        => new InterfaceBuilder()
            .WithName("IMyClass")
            .WithNamespace("MySourceNamespace")
            .AddProperties(
                new PropertyBuilder().WithName("Property1").WithType(typeof(int)),
                new PropertyBuilder().WithName("Property2").WithType(typeof(int)).WithIsNullable(),
                new PropertyBuilder().WithName("Property3").WithType(typeof(string)),
                new PropertyBuilder().WithName("Property4").WithType(typeof(string)).WithIsNullable(),
                new PropertyBuilder().WithName("Property5").WithTypeName("MySourceNamespace.IMyClass"),
                new PropertyBuilder().WithName("Property6").WithTypeName("MySourceNamespace.IMyClass").WithIsNullable(),
                new PropertyBuilder().WithName("Property7").WithTypeName(typeof(List<>).ReplaceGenericTypeName("MySourceNamespace.IMyClass")),
                new PropertyBuilder().WithName("Property8").WithTypeName(typeof(List<>).ReplaceGenericTypeName("MySourceNamespace.IMyClass")).WithIsNullable())
            .BuildTyped();

    protected static Class CreateClassWithPropertyThatHasAReservedName(Type propertyType)
        => new ClassBuilder()
            .WithName("SomeClass")
            .WithNamespace("SomeNamespace")
            .AddProperties(new PropertyBuilder().WithName("Delegate").WithType(propertyType))
            .BuildTyped();

    protected static Property CreateProperty()
        => new PropertyBuilder().WithName("MyProperty").WithType(typeof(string)).Build();

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
        bool createAsObservable = false,
        bool inheritFromInterfaces = false,
        string newCollectionTypeName = "System.Collections.Generic.List",
        IEnumerable<NamespaceMappingBuilder>? namespaceMappings = null,
        IEnumerable<TypenameMappingBuilder>? typenameMappings = null,
        string setMethodNameFormatString = "With{$property.Name}",
        string addMethodNameFormatString = "Add{$property.Name}",
        string builderNamespaceFormatString = "{$class.Namespace}.Builders",
        string builderNameFormatString = "{$class.Name}Builder",
        string buildMethodName = "Build",
        string buildTypedMethodName = "BuildTyped",
        string setDefaultValuesMethodName = "SetDefaultValues",
        ArgumentValidationType validateArguments = ArgumentValidationType.None,
        string? baseClassBuilderNameSpace = null,
        bool allowGenerationWithoutProperties = false,
        Class? baseClass = null,
        InheritanceComparisonDelegate? inheritanceComparisonDelegate = null,
        Predicate<Domain.Attribute>? copyAttributePredicate = null,
        Predicate<string>? copyInterfacePredicate = null)
        => CreateSettingsForEntity
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
                inheritFromInterfaces: inheritFromInterfaces,
                createAsObservable: createAsObservable
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
        string entityNamespaceFormatString = "{$class.Namespace}",
        string entityNameFormatString = "{$class.Name}",
        string toBuilderFormatString = "ToBuilder",
        string toTypedBuilderFormatString = "ToTypedBuilder",
        string newCollectionTypeName = "System.Collections.Generic.IReadOnlyCollection",
        string collectionTypeName = "",
        bool addSetters = false,
        bool createRecord = false,
        bool addBackingFields = false,
        bool createAsObservable = false,
        bool inheritFromInterfaces = false,
        bool implementIEquatable = false,
        bool usePatternMatchingForNullChecks = true,
        IEquatableItemType iEquatableItemType = IEquatableItemType.Properties,
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
            .WithImplementIEquatable(implementIEquatable)
            .WithIEquatableItemType(iEquatableItemType)
            .WithUseExceptionThrowIfNull(useExceptionThrowIfNull)
            .WithEnableInheritance(enableEntityInheritance)
            .WithIsAbstract(isAbstract)
            .WithBaseClass(baseClass?.ToBuilder())
            .WithValidateArguments(validateArguments)
            .WithCollectionTypeName(collectionTypeName)
            .WithAddFullConstructor(addFullConstructor)
            .WithUsePatternMatchingForNullChecks(usePatternMatchingForNullChecks)
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
        string entityNamespaceFormatString = "{$class.Namespace}",
        string entityNameFormatString = "{$class.Name}",
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
        string namespaceFormatString = "{$class.Namespace}",
        string nameFormatString = "{$class.Name}",
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
        string namespaceFormatString = "{$class.Namespace}",
        string nameFormatString = "{$class.Name}",
        string newCollectionTypeName = "System.Collections.Generic.IReadOnlyCollection",
        Class? baseClass = null,
        IEnumerable<NamespaceMappingBuilder>? namespaceMappings = null,
        IEnumerable<TypenameMappingBuilder>? typenameMappings = null,
        Predicate<Domain.Attribute>? copyAttributePredicate = null,
        Predicate<string>? copyInterfacePredicate = null,
        CopyMethodPredicate? copyMethodPredicate = null)
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
                .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderName).WithValue("{ClassName($property.TypeName)}Builder"))
                .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomEntityNamespace).WithValue("MyNamespace"))
                .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderSourceExpression).WithValue("[Name][NullableSuffix].ToBuilder()[ForcedNullableSuffix]"))
                .AddMetadata(new MetadataBuilder().WithName(MetadataNames.CustomBuilderMethodParameterExpression).WithValue("[Name][NullableSuffix].Build()[ForcedNullableSuffix]"))
        ];

    protected static IEnumerable<TypenameMappingBuilder> CreateTypenameMappings()
        =>
        [
            new TypenameMappingBuilder().WithSourceTypeName(typeof(List<>).WithoutGenerics()).WithTargetTypeName(typeof(List<>).WithoutGenerics()).AddMetadata(MetadataNames.CustomCollectionInitialization, "new [Type][Generics]([Expression])"), //"[Expression].ToList()"
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
                    new MetadataBuilder().WithValue("{ClassName($property.TypeName)}Builder").WithName(MetadataNames.CustomBuilderName),
                    new MetadataBuilder().WithValue("new ExpressionFramework.Domain.Builders.Evaluatables.ComposedEvaluatableBuilder(source.[Name])").WithName(MetadataNames.CustomBuilderConstructorInitializeExpression),
                    new MetadataBuilder().WithValue(new Literal("new ExpressionFramework.Domain.Builders.Evaluatables.ComposedEvaluatableBuilder()", null)).WithName(MetadataNames.CustomBuilderDefaultValue),
                    new MetadataBuilder().WithValue("[Name][NullableSuffix].BuildTyped()[ForcedNullableSuffix]").WithName(MetadataNames.CustomBuilderMethodParameterExpression)
                ),
            new TypenameMappingBuilder()
                .WithSourceTypeName("ExpressionFramework.Domain.Expression")
                .WithTargetTypeName("ExpressionFramework.Domain.Expression")
                .AddMetadata
                (
                    new MetadataBuilder().WithValue("ExpressionFramework.Domain.Builders").WithName(MetadataNames.CustomBuilderNamespace),
                    new MetadataBuilder().WithValue("{ClassName($property.TypeName)}Builder").WithName(MetadataNames.CustomBuilderName),
                    new MetadataBuilder().WithValue("ExpressionFramework.Domain.Builders.ExpressionBuilderFactory.Create(source.[Name])").WithName(MetadataNames.CustomBuilderConstructorInitializeExpression),
                    new MetadataBuilder().WithValue(new Literal("default(ExpressionFramework.Domain.Builders.ExpressionBuilder)!", null)).WithName(MetadataNames.CustomBuilderDefaultValue),
                    new MetadataBuilder().WithValue($"[Name][NullableSuffix].Build()[ForcedNullableSuffix]").WithName(MetadataNames.CustomBuilderMethodParameterExpression)
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
        if (propInfo is not null && propInfo.DeclaringType?.Name.EndsWith("Builder", StringComparison.Ordinal) == true)
        {
            return new OmitSpecimen();
        }

        return new NoSpecimen();
    }
}
