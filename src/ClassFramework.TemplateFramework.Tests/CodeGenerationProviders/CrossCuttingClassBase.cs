namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class CrossCuttingClassBase(ICommandService commandService) : CsharpClassGeneratorPipelineCodeGenerationProviderBase(commandService)
{
    public override bool RecurseOnDeleteGeneratedFiles => false;
    public override string LastGeneratedFilesFilename => string.Empty;
    public override Encoding Encoding => Encoding.UTF8;

    protected override Type EntityCollectionType => typeof(IReadOnlyCollection<>);
    protected override Type EntityConcreteCollectionType => typeof(List<>);
    protected override Type BuilderCollectionType => typeof(List<>);

    protected override string ProjectName => "CrossCutting";
    protected override string CoreNamespace => "CrossCutting.Utilities.Parsers";
    protected override bool CopyAttributes => true;
    protected override bool CopyInterfaces => true;
    protected override bool CreateRecord => true;
    protected override bool EnableGlobalUsings => true;
    protected override bool CreateCodeGenerationHeader => false;

    protected override bool IsAbstractType(Type type)
    {
        Guard.IsNotNull(type);

        return type.IsInterface && type.Name.WithoutTypeGenerics().EndsWith("Base");
    }

    protected static Task<Result<IEnumerable<TypeBase>>> GetCrossCuttingAbstractModelsAsync()
    {
        var modelsResult = GetCrossCuttingModels().Where(x => x.Name.EndsWith("Base")).Select(x => Result.Success(x.Build())).ToArray();

        return Task.FromResult(Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create abstract models. See the inner results for more details.")));
    }

    protected Task<Result<IEnumerable<TypeBase>>> GetCrossCuttingAbstractionsInterfacesAsync()
    {
        var modelsResult = GetCrossCuttingModels().Where(x => x.Namespace == $"{CoreNamespace}.Abstractions").Select(x => Result.Success(x.Build())).ToArray();

        return Task.FromResult(Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create abstract interfaces. See the inner results for more details.")));
    }

    protected static Task<Result<IEnumerable<TypeBase>>> GetCrossCuttingOverrideModelsAsync(string abstractTypeName)
    {
        Guard.IsNotNull(abstractTypeName);

        var modelsResult = GetCrossCuttingModels().Where(x => x.Interfaces.Any(y => y.WithoutGenerics() == abstractTypeName.WithoutGenerics())).Select(x => Result.Success(x.Build())).ToArray();

        return Task.FromResult(Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create override models. See the inner results for more details.")));
    }

    protected async Task<Result<TypeBase>> CreateCrossCuttingBaseClassAsync(string typeName, string @namespace)
    {
        Guard.IsNotNull(typeName);
        Guard.IsNotNull(@namespace);

        return await ProcessCrossCuttingBaseClassResultAsync(Task.FromResult(Result.Success(GetCrossCuttingModels().Single(x => x.GetFullName() == typeName).Build())), async typeBaseResult =>
        {
            var entitySettings = new PipelineSettingsBuilder()
                .WithAddSetters(AddSetters)
                .WithAddBackingFields(AddBackingFields)
                .WithSetterVisibility(SetterVisibility)
                .WithCreateAsObservable(CreateAsObservable)
                .WithCreateRecord(CreateRecord)
                .WithAllowGenerationWithoutProperties(AllowGenerationWithoutProperties)
                .WithCopyAttributes(CopyAttributes)
                .WithCopyInterfaces(CopyInterfaces)
                .WithCopyMethods(CopyMethods)
                .WithCopyAttributePredicate(CopyAttributePredicate)
                .WithCopyInterfacePredicate(CopyInterfacePredicate)
                .WithCopyMethodPredicate(CopyMethodPredicate)
                .WithEntityNameFormatString("{NoInterfacePrefix(class.Name)}")
                .WithEntityNamespaceFormatString(@namespace)
                .WithEnableInheritance()
                .WithIsAbstract()
                .WithBaseClass(null)
                .WithInheritanceComparisonDelegate(
                   (parentNameContainer, typeBase)
                       => parentNameContainer is not null
                       && typeBase is not null
                       && (string.IsNullOrEmpty(parentNameContainer.ParentTypeFullName)
                           || parentNameContainer.ParentTypeFullName.GetClassName().In(typeBase.Name, $"I{typeBase.Name}")
                           || Array.Exists(GetModelAbstractBaseTyped(), x => x == parentNameContainer.ParentTypeFullName.GetClassName())
                           || (parentNameContainer.ParentTypeFullName.StartsWith($"{RootNamespace}.") && typeBase.Namespace.In(CoreNamespace, $"{RootNamespace}.Builders"))
                       ))
                .WithEntityNewCollectionTypeName(EntityCollectionType.WithoutGenerics())
                .WithEnableNullableReferenceTypes()
                .AddTypenameMappings(CreateTypenameMappings())
                .AddNamespaceMappings(CreateNamespaceMappings())
                .WithValidateArguments(ValidateArgumentsInConstructor)
                .WithAddNullChecks(AddNullChecks)
                .WithUseExceptionThrowIfNull(UseExceptionThrowIfNull);

            return (await CommandService.ExecuteAsync<EntityContext, ClassBuilder>(new EntityContext(typeBaseResult!, entitySettings, Settings.CultureInfo, CancellationToken.None)).ConfigureAwait(false))
                .OnSuccess(x => x.Build());
        }).ConfigureAwait(false);
    }

    protected static IEnumerable<TypeBaseBuilder> GetCrossCuttingModels()
    {
#pragma warning disable CA1861 // Avoid constant arrays as arguments
        return new[]
        {
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Interfaces = new ObservableCollection<System.String>(),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"FormatProvider",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.IFormatProvider",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IExpressionStringEvaluatorSettings",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"ValidateArgumentTypes",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Boolean",
                        IsValueType = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IExpressionStringEvaluatorSettings",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IExpressionStringEvaluatorSettings",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Interfaces = new ObservableCollection<System.String>(),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"FormatProvider",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.IFormatProvider",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFormattableStringParserSettings",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"ValidateArgumentTypes",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Boolean",
                        IsValueType = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFormattableStringParserSettings",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"PlaceholderStart",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.String",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFormattableStringParserSettings",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"PlaceholderEnd",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.String",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFormattableStringParserSettings",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"EscapeBraces",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Boolean",
                        IsValueType = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFormattableStringParserSettings",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"MaximumRecursion",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Int32",
                        IsValueType = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFormattableStringParserSettings",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IFormattableStringParserSettings",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Interfaces = new ObservableCollection<System.String>(),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Name",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.String",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionCall",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Arguments",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Collections.Generic.IReadOnlyCollection<CrossCutting.CodeGeneration.Models.Abstractions.IFunctionCallArgument>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                        {
                            new PropertyBuilder
                            {
                                GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                Name = @"Dummy",
                                Attributes = new ObservableCollection<AttributeBuilder>(),
                                TypeName = @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                                GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                            },
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionCall",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IFunctionCall",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IFunctionCallArgumentBase",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Interfaces = new ObservableCollection<System.String>(),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Name",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.String",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptor",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"FunctionType",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Type",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptor",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"ReturnValueType",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Type",
                        IsNullable = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptor",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Description",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.String",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptor",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Arguments",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Collections.Generic.IReadOnlyCollection<CrossCutting.CodeGeneration.Models.IFunctionDescriptorArgument>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                        {
                            new PropertyBuilder
                            {
                                GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                Name = @"Dummy",
                                Attributes = new ObservableCollection<AttributeBuilder>(),
                                TypeName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorArgument",
                                GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                            },
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptor",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Results",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Collections.Generic.IReadOnlyCollection<CrossCutting.CodeGeneration.Models.IFunctionDescriptorResult>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                        {
                            new PropertyBuilder
                            {
                                GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                Name = @"Dummy",
                                Attributes = new ObservableCollection<AttributeBuilder>(),
                                TypeName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorResult",
                                GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                            },
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptor",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IFunctionDescriptor",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Interfaces = new ObservableCollection<System.String>(),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Name",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.String",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorArgument",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Type",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Type",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorArgument",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Description",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.String",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorArgument",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"IsRequired",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Boolean",
                        IsValueType = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorArgument",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IFunctionDescriptorArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Interfaces = new ObservableCollection<System.String>(),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Status",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"CrossCutting.Common.Results.ResultStatus",
                        IsValueType = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorResult",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Value",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.String",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorResult",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"ValueType",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Type",
                        IsNullable = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorResult",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Description",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.String",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorResult",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IFunctionDescriptorResult",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Interfaces = new ObservableCollection<System.String>(),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"FormatProvider",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.IFormatProvider",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionEvaluatorSettings",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"ValidateArgumentTypes",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Boolean",
                        IsValueType = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionEvaluatorSettings",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IFunctionEvaluatorSettings",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Interfaces = new ObservableCollection<System.String>(),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"FormatProvider",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.IFormatProvider",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IPlaceholderSettings",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"ValidateArgumentTypes",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Boolean",
                        IsValueType = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IPlaceholderSettings",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IPlaceholderSettings",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Value",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Object",
                        IsNullable = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IConstantArgument",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IConstantArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Value",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"T",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IConstantArgument`1",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IConstantArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(new[]
                {
                    @"T",
                } ),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Result",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"CrossCutting.Common.Results.Result<System.Object?>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                        {
                            new PropertyBuilder
                            {
                                GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                Name = @"Dummy",
                                Attributes = new ObservableCollection<AttributeBuilder>(),
                                TypeName = @"System.Object",
                                GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                            },
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IConstantResultArgument",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IConstantResultArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Result",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"CrossCutting.Common.Results.Result<T>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                        {
                            new PropertyBuilder
                            {
                                GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                Name = @"Dummy",
                                Attributes = new ObservableCollection<AttributeBuilder>(),
                                TypeName = @"T",
                                GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                            },
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IConstantResultArgument`1",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IConstantResultArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(new[]
                {
                    @"T",
                } ),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Delegate",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Func<System.Object?>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                        {
                            new PropertyBuilder
                            {
                                GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                Name = @"Dummy",
                                Attributes = new ObservableCollection<AttributeBuilder>(),
                                TypeName = @"System.Object",
                                GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                            },
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateArgument",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"ValidationDelegate",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Func<System.Type>?",
                        IsNullable = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                        {
                            new PropertyBuilder
                            {
                                GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                Name = @"Dummy",
                                Attributes = new ObservableCollection<AttributeBuilder>(),
                                TypeName = @"System.Type",
                                GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                            },
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateArgument",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IDelegateArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Delegate",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Func<T>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                        {
                            new PropertyBuilder
                            {
                                GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                Name = @"Dummy",
                                Attributes = new ObservableCollection<AttributeBuilder>(),
                                TypeName = @"T",
                                GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                            },
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateArgument`1",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"ValidationDelegate",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Func<System.Type>?",
                        IsNullable = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                        {
                            new PropertyBuilder
                            {
                                GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                Name = @"Dummy",
                                Attributes = new ObservableCollection<AttributeBuilder>(),
                                TypeName = @"System.Type",
                                GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                            },
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateArgument`1",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IDelegateArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(new[]
                {
                    @"T",
                } ),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Delegate",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Func<CrossCutting.Common.Results.Result<System.Object?>>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                        {
                            new PropertyBuilder
                            {
                                GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                Name = @"Dummy",
                                Attributes = new ObservableCollection<AttributeBuilder>(),
                                TypeName = @"CrossCutting.Common.Results.Result<System.Object?>",
                                GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                                {
                                    new PropertyBuilder
                                    {
                                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                        Name = @"Dummy",
                                        Attributes = new ObservableCollection<AttributeBuilder>(),
                                        TypeName = @"System.Object",
                                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                                    },
                                } ),
                            },
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateResultArgument",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"ValidationDelegate",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Func<CrossCutting.Common.Results.Result<System.Type>>?",
                        IsNullable = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                        {
                            new PropertyBuilder
                            {
                                GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                Name = @"Dummy",
                                Attributes = new ObservableCollection<AttributeBuilder>(),
                                TypeName = @"CrossCutting.Common.Results.Result<System.Type>",
                                GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                                {
                                    new PropertyBuilder
                                    {
                                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                        Name = @"Dummy",
                                        Attributes = new ObservableCollection<AttributeBuilder>(),
                                        TypeName = @"System.Type",
                                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                                    },
                                } ),
                            },
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateResultArgument",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IDelegateResultArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Delegate",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Func<CrossCutting.Common.Results.Result<T>>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                        {
                            new PropertyBuilder
                            {
                                GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                Name = @"Dummy",
                                Attributes = new ObservableCollection<AttributeBuilder>(),
                                TypeName = @"CrossCutting.Common.Results.Result<T>",
                                GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                                {
                                    new PropertyBuilder
                                    {
                                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                        Name = @"Dummy",
                                        Attributes = new ObservableCollection<AttributeBuilder>(),
                                        TypeName = @"T",
                                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                                    },
                                } ),
                            },
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateResultArgument`1",
                    },
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"ValidationDelegate",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.Func<CrossCutting.Common.Results.Result<System.Type>>?",
                        IsNullable = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                        {
                            new PropertyBuilder
                            {
                                GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                Name = @"Dummy",
                                Attributes = new ObservableCollection<AttributeBuilder>(),
                                TypeName = @"CrossCutting.Common.Results.Result<System.Type>",
                                GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(new[]
                                {
                                    new PropertyBuilder
                                    {
                                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                                        Name = @"Dummy",
                                        Attributes = new ObservableCollection<AttributeBuilder>(),
                                        TypeName = @"System.Type",
                                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                                    },
                                } ),
                            },
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateResultArgument`1",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IDelegateResultArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(new[]
                {
                    @"T",
                } ),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IEmptyArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IEmptyArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(new[]
                {
                    @"T",
                } ),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Value",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"System.String",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IExpressionArgument",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IExpressionArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasSetter = false,
                        GetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        SetterCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        InitializerCodeStatements = new ObservableCollection<CodeStatementBaseBuilder>(),
                        Name = @"Function",
                        Attributes = new ObservableCollection<AttributeBuilder>(),
                        TypeName = @"CrossCutting.Utilities.Parsers.IFunctionCall",
                        GenericTypeArguments = new ObservableCollection<ITypeContainerBuilder>(),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IFunctionArgument",
                    },
                } ),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IFunctionArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.Abstractions",
                Interfaces = new ObservableCollection<System.String>(),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IFunctionCallArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.Abstractions",
                Interfaces = new ObservableCollection<System.String>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                } ),
                Fields = new ObservableCollection<FieldBuilder>(),
                Properties = new ObservableCollection<PropertyBuilder>(),
                Methods = new ObservableCollection<MethodBuilder>(),
                Visibility = ClassFramework.Domain.Domains.Visibility.Internal,
                Name = @"IFunctionCallArgument",
                Attributes = new ObservableCollection<AttributeBuilder>(),
                GenericTypeArguments = new ObservableCollection<System.String>(new[]
                {
                    @"T",
                } ),
                GenericTypeArgumentConstraints = new ObservableCollection<System.String>(),
                SuppressWarningCodes = new ObservableCollection<System.String>(),
            },
        };
#pragma warning restore CA1861 // Avoid constant arrays as arguments
    }

    private static async Task<Result<T>> ProcessCrossCuttingBaseClassResultAsync<T>(Task<Result<TypeBase>> baseClassResultTask, Func<TypeBase?, Task<Result<T>>> successTask)
    {
        Guard.IsNotNull(baseClassResultTask);
        Guard.IsNotNull(successTask);

        var baseClassResult = await baseClassResultTask.ConfigureAwait(false);
        if (!baseClassResult.IsSuccessful() && baseClassResult.Status != ResultStatus.NotFound)
        {
            return Result.Error<T>([baseClassResult], "Could not get base class, see inner results for details");
        }

        return await successTask(baseClassResult.Value).ConfigureAwait(false);
    }
}
