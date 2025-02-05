namespace ClassFramework.TemplateFramework.Tests.CodeGenerationProviders;

public abstract class CrossCuttingClassBase(IPipelineService pipelineService) : CsharpClassGeneratorPipelineCodeGenerationProviderBase(pipelineService)
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
    protected override bool UseBuilderAbstractionsTypeConversion => true;

    protected override bool IsAbstractType(Type type)
    {
        Guard.IsNotNull(type);

        return type.IsInterface && type.Name.WithoutTypeGenerics().EndsWith("Base");
    }

    protected Task<Result<IEnumerable<TypeBase>>> GetCrossCuttingAbstractModels()
    {
        var modelsResult = GetCrossCuttingModels().Where(x => x.Name.EndsWith("Base")).Select(x => Result.Success(x.Build())).ToArray();

        return Task.FromResult(Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create abstract models. See the inner results for more details.")));
    }

    protected Task<Result<IEnumerable<TypeBase>>> GetCrossCuttingAbstractionsInterfaces()
    {
        var modelsResult = GetCrossCuttingModels().Where(x => x.Namespace == $"{CoreNamespace}.Abstractions").Select(x => Result.Success(x.Build())).ToArray();

        return Task.FromResult(Result.Aggregate(modelsResult, Result.Success(modelsResult.Select(x => x.Value!)), x => Result.Error<IEnumerable<TypeBase>>(x, "Could not create abstract interfaces. See the inner results for more details.")));
    }

    protected IEnumerable<TypeBaseBuilder> GetCrossCuttingModels()
    {
#pragma warning disable CA1861 // Avoid constant arrays as arguments
        return new List<TypeBaseBuilder>(new[]
        {
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"FormatProvider",
                        TypeName = @"System.IFormatProvider",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IExpressionStringEvaluatorSettings",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"ValidateArgumentTypes",
                        TypeName = @"System.Boolean",
                        IsValueType = true,
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IExpressionStringEvaluatorSettings",
                    },
                } ),
                Visibility = Visibility.Internal,
                Name = @"IExpressionStringEvaluatorSettings",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"FormatProvider",
                        TypeName = @"System.IFormatProvider",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFormattableStringParserSettings",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"ValidateArgumentTypes",
                        TypeName = @"System.Boolean",
                        IsValueType = true,
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFormattableStringParserSettings",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"PlaceholderStart",
                        TypeName = @"System.String",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFormattableStringParserSettings",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"PlaceholderEnd",
                        TypeName = @"System.String",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFormattableStringParserSettings",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"EscapeBraces",
                        TypeName = @"System.Boolean",
                        IsValueType = true,
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFormattableStringParserSettings",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"MaximumRecursion",
                        TypeName = @"System.Int32",
                        IsValueType = true,
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFormattableStringParserSettings",
                    },
                } ),
                Visibility = Visibility.Internal,
                Name = @"IFormattableStringParserSettings",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Name",
                        TypeName = @"System.String",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionCall",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Arguments",
                        TypeName = @"System.Collections.Generic.IReadOnlyCollection<CrossCutting.CodeGeneration.Models.Abstractions.IFunctionCallArgument>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainer>(new[]
                        {
                            new Property
                            (
                                hasGetter: true,
                                hasSetter: true,
                                hasInitializer: false,
                                getterVisibility: SubVisibility.InheritFromParent,
                                setterVisibility: SubVisibility.InheritFromParent,
                                initializerVisibility: SubVisibility.InheritFromParent,
                                getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                @static: false,
                                @virtual: false,
                                @abstract: false,
                                @protected: false,
                                @override: false,
                                @new: false,
                                visibility: Visibility.Public,
                                name: @"Dummy",
                                attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                typeName: @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                                isNullable: false,
                                isValueType: false,
                                genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(),
                                defaultValue: null,
                                explicitInterfaceName: @"",
                                parentTypeFullName: @""
                            ),
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionCall",
                    },
                } ),
                Visibility = Visibility.Internal,
                Name = @"IFunctionCall",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                }),
                Visibility = Visibility.Internal,
                Name = @"IFunctionCallArgumentBase",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Name",
                        TypeName = @"System.String",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptor",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"FunctionType",
                        TypeName = @"System.Type",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptor",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"ReturnValueType",
                        TypeName = @"System.Type",
                        IsNullable = true,
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptor",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Description",
                        TypeName = @"System.String",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptor",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Arguments",
                        TypeName = @"System.Collections.Generic.IReadOnlyCollection<CrossCutting.CodeGeneration.Models.IFunctionDescriptorArgument>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainer>(new[]
                        {
                            new Property
                            (
                                hasGetter: true,
                                hasSetter: true,
                                hasInitializer: false,
                                getterVisibility: SubVisibility.InheritFromParent,
                                setterVisibility: SubVisibility.InheritFromParent,
                                initializerVisibility: SubVisibility.InheritFromParent,
                                getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                @static: false,
                                @virtual: false,
                                @abstract: false,
                                @protected: false,
                                @override: false,
                                @new: false,
                                visibility: Visibility.Public,
                                name: @"Dummy",
                                attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                typeName: @"CrossCutting.Utilities.Parsers.IFunctionDescriptorArgument",
                                isNullable: false,
                                isValueType: false,
                                genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(),
                                defaultValue: null,
                                explicitInterfaceName: @"",
                                parentTypeFullName: @""
                            ),
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptor",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Results",
                        TypeName = @"System.Collections.Generic.IReadOnlyCollection<CrossCutting.CodeGeneration.Models.IFunctionDescriptorResult>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainer>(new[]
                        {
                            new Property
                            (
                                hasGetter: true,
                                hasSetter: true,
                                hasInitializer: false,
                                getterVisibility: SubVisibility.InheritFromParent,
                                setterVisibility: SubVisibility.InheritFromParent,
                                initializerVisibility: SubVisibility.InheritFromParent,
                                getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                @static: false,
                                @virtual: false,
                                @abstract: false,
                                @protected: false,
                                @override: false,
                                @new: false,
                                visibility: Visibility.Public,
                                name: @"Dummy",
                                attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                typeName: @"CrossCutting.Utilities.Parsers.IFunctionDescriptorResult",
                                isNullable: false,
                                isValueType: false,
                                genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(),
                                defaultValue: null,
                                explicitInterfaceName: @"",
                                parentTypeFullName: @""
                            ),
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptor",
                    },
                } ),
                Visibility = Visibility.Internal,
                Name = @"IFunctionDescriptor",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Name",
                        TypeName = @"System.String",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorArgument",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Type",
                        TypeName = @"System.Type",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorArgument",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Description",
                        TypeName = @"System.String",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorArgument",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"IsRequired",
                        TypeName = @"System.Boolean",
                        IsValueType = true,
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorArgument",
                    },
                }),
                Visibility = Visibility.Internal,
                Name = @"IFunctionDescriptorArgument",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Status",
                        TypeName = @"CrossCutting.Common.Results.ResultStatus",
                        IsValueType = true,
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorResult",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Value",
                        TypeName = @"System.String",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorResult",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"ValueType",
                        TypeName = @"System.Type",
                        IsNullable = true,
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorResult",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Description",
                        TypeName = @"System.String",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionDescriptorResult",
                    },
                }),
                Visibility = Visibility.Internal,
                Name = @"IFunctionDescriptorResult",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"FormatProvider",
                        TypeName = @"System.IFormatProvider",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionEvaluatorSettings",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"ValidateArgumentTypes",
                        TypeName = @"System.Boolean",
                        IsValueType = true,
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IFunctionEvaluatorSettings",
                    },
                }),
                Visibility = Visibility.Internal,
                Name = @"IFunctionEvaluatorSettings",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers",
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"FormatProvider",
                        TypeName = @"System.IFormatProvider",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IPlaceholderSettings",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"ValidateArgumentTypes",
                        TypeName = @"System.Boolean",
                        IsValueType = true,
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.IPlaceholderSettings",
                    },
                }),
                Visibility = Visibility.Internal,
                Name = @"IPlaceholderSettings",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                }),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Value",
                        TypeName = @"System.Object",
                        IsNullable = true,
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IConstantArgument",
                    },
                }),
                Visibility = Visibility.Internal,
                Name = @"IConstantArgument",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>",
                }),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Value",
                        TypeName = @"T",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IConstantArgument`1",
                    },
                }),
                Visibility = Visibility.Internal,
                Name = @"IConstantArgument",
                GenericTypeArguments = new ObservableCollection<string>(new[]
                {
                    @"T",
                }),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                }),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Result",
                        TypeName = @"CrossCutting.Common.Results.Result<System.Object?>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainer>(new[]
                        {
                            new Property
                            (
                                hasGetter: true,
                                hasSetter: true,
                                hasInitializer: false,
                                getterVisibility: SubVisibility.InheritFromParent,
                                setterVisibility: SubVisibility.InheritFromParent,
                                initializerVisibility: SubVisibility.InheritFromParent,
                                getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                @static: false,
                                @virtual: false,
                                @abstract: false,
                                @protected: false,
                                @override: false,
                                @new: false,
                                visibility: Visibility.Public,
                                name: @"Dummy",
                                attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                typeName: @"System.Object",
                                isNullable: false,
                                isValueType: false,
                                genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(),
                                defaultValue: null,
                                explicitInterfaceName: @"",
                                parentTypeFullName: @""
                            ),
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IConstantResultArgument",
                    },
                } ),
                Visibility = Visibility.Internal,
                Name = @"IConstantResultArgument",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>",
                }),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Result",
                        TypeName = @"CrossCutting.Common.Results.Result<T>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainer>(new[]
                        {
                            new Property
                            (
                                hasGetter: true,
                                hasSetter: true,
                                hasInitializer: false,
                                getterVisibility: SubVisibility.InheritFromParent,
                                setterVisibility: SubVisibility.InheritFromParent,
                                initializerVisibility: SubVisibility.InheritFromParent,
                                getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                @static: false,
                                @virtual: false,
                                @abstract: false,
                                @protected: false,
                                @override: false,
                                @new: false,
                                visibility: Visibility.Public,
                                name: @"Dummy",
                                attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                typeName: @"T",
                                isNullable: false,
                                isValueType: false,
                                genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(),
                                defaultValue: null,
                                explicitInterfaceName: @"",
                                parentTypeFullName: @""
                            ),
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IConstantResultArgument`1",
                    },
                } ),
                Visibility = Visibility.Internal,
                Name = @"IConstantResultArgument",
                GenericTypeArguments = new ObservableCollection<string>(new[]
                {
                    @"T",
                }),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                }),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Delegate",
                        TypeName = @"System.Func<System.Object?>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainer>(new[]
                        {
                            new Property
                            (
                                hasGetter: true,
                                hasSetter: true,
                                hasInitializer: false,
                                getterVisibility: SubVisibility.InheritFromParent,
                                setterVisibility: SubVisibility.InheritFromParent,
                                initializerVisibility: SubVisibility.InheritFromParent,
                                getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                @static: false,
                                @virtual: false,
                                @abstract: false,
                                @protected: false,
                                @override: false,
                                @new: false,
                                visibility: Visibility.Public,
                                name: @"Dummy",
                                attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                typeName: @"System.Object",
                                isNullable: false,
                                isValueType: false,
                                genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(),
                                defaultValue: null,
                                explicitInterfaceName: @"",
                                parentTypeFullName: @""
                            ),
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateArgument",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"ValidationDelegate",
                        TypeName = @"System.Func<System.Type>?",
                        IsNullable = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainer>(new[]
                        {
                            new Property
                            (
                                hasGetter: true,
                                hasSetter: true,
                                hasInitializer: false,
                                getterVisibility: SubVisibility.InheritFromParent,
                                setterVisibility: SubVisibility.InheritFromParent,
                                initializerVisibility: SubVisibility.InheritFromParent,
                                getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                @static: false,
                                @virtual: false,
                                @abstract: false,
                                @protected: false,
                                @override: false,
                                @new: false,
                                visibility: Visibility.Public,
                                name: @"Dummy",
                                attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                typeName: @"System.Type",
                                isNullable: false,
                                isValueType: false,
                                genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(),
                                defaultValue: null,
                                explicitInterfaceName: @"",
                                parentTypeFullName: @""
                            ),
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateArgument",
                    },
                } ),
                Visibility = Visibility.Internal,
                Name = @"IDelegateArgument",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>",
                }),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Delegate",
                        TypeName = @"System.Func<T>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainer>(new[]
                        {
                            new Property
                            (
                                hasGetter: true,
                                hasSetter: true,
                                hasInitializer: false,
                                getterVisibility: SubVisibility.InheritFromParent,
                                setterVisibility: SubVisibility.InheritFromParent,
                                initializerVisibility: SubVisibility.InheritFromParent,
                                getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                @static: false,
                                @virtual: false,
                                @abstract: false,
                                @protected: false,
                                @override: false,
                                @new: false,
                                visibility: Visibility.Public,
                                name: @"Dummy",
                                attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                typeName: @"T",
                                isNullable: false,
                                isValueType: false,
                                genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(),
                                defaultValue: null,
                                explicitInterfaceName: @"",
                                parentTypeFullName: @""
                            ),
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateArgument`1",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"ValidationDelegate",
                        TypeName = @"System.Func<System.Type>?",
                        IsNullable = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainer>(new[]
                        {
                            new Property
                            (
                                hasGetter: true,
                                hasSetter: true,
                                hasInitializer: false,
                                getterVisibility: SubVisibility.InheritFromParent,
                                setterVisibility: SubVisibility.InheritFromParent,
                                initializerVisibility: SubVisibility.InheritFromParent,
                                getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                @static: false,
                                @virtual: false,
                                @abstract: false,
                                @protected: false,
                                @override: false,
                                @new: false,
                                visibility: Visibility.Public,
                                name: @"Dummy",
                                attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                typeName: @"System.Type",
                                isNullable: false,
                                isValueType: false,
                                genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(),
                                defaultValue: null,
                                explicitInterfaceName: @"",
                                parentTypeFullName: @""
                            ),
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateArgument`1",
                    },
                } ),
                Visibility = Visibility.Internal,
                Name = @"IDelegateArgument",
                GenericTypeArguments = new ObservableCollection<string>(new[]
                {
                    @"T",
                }),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                }),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Delegate",
                        TypeName = @"System.Func<CrossCutting.Common.Results.Result<System.Object?>>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainer>(new[]
                        {
                            new Property
                            (
                                hasGetter: true,
                                hasSetter: true,
                                hasInitializer: false,
                                getterVisibility: SubVisibility.InheritFromParent,
                                setterVisibility: SubVisibility.InheritFromParent,
                                initializerVisibility: SubVisibility.InheritFromParent,
                                getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                @static: false,
                                @virtual: false,
                                @abstract: false,
                                @protected: false,
                                @override: false,
                                @new: false,
                                visibility: Visibility.Public,
                                name: @"Dummy",
                                attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                typeName: @"CrossCutting.Common.Results.Result<System.Object?>",
                                isNullable: false,
                                isValueType: false,
                                genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(new[]
                                {
                                    new Property
                                    (
                                        hasGetter: true,
                                        hasSetter: true,
                                        hasInitializer: false,
                                        getterVisibility: SubVisibility.InheritFromParent,
                                        setterVisibility: SubVisibility.InheritFromParent,
                                        initializerVisibility: SubVisibility.InheritFromParent,
                                        getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(                                ),
                                        setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(                                ),
                                        initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(                                ),
                                        @static: false,
                                        @virtual: false,
                                        @abstract: false,
                                        @protected: false,
                                        @override: false,
                                        @new: false,
                                        visibility: Visibility.Public,
                                        name: @"Dummy",
                                        attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                        typeName: @"System.Object",
                                        isNullable: false,
                                        isValueType: false,
                                        genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(),
                                        defaultValue: null,
                                        explicitInterfaceName: @"",
                                        parentTypeFullName: @""
                                    ),
                                } ),
                                defaultValue: null,
                                explicitInterfaceName: @"",
                                parentTypeFullName: @""
                            ),
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateResultArgument",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"ValidationDelegate",
                        TypeName = @"System.Func<CrossCutting.Common.Results.Result<System.Type>>?",
                        IsNullable = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainer>(new[]
                        {
                            new Property
                            (
                                hasGetter: true,
                                hasSetter: true,
                                hasInitializer: false,
                                getterVisibility: SubVisibility.InheritFromParent,
                                setterVisibility: SubVisibility.InheritFromParent,
                                initializerVisibility: SubVisibility.InheritFromParent,
                                getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                @static: false,
                                @virtual: false,
                                @abstract: false,
                                @protected: false,
                                @override: false,
                                @new: false,
                                visibility: Visibility.Public,
                                name: @"Dummy",
                                attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                typeName: @"CrossCutting.Common.Results.Result<System.Type>",
                                isNullable: false,
                                isValueType: false,
                                genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(new[]
                                {
                                    new Property
                                    (
                                        hasGetter: true,
                                        hasSetter: true,
                                        hasInitializer: false,
                                        getterVisibility: SubVisibility.InheritFromParent,
                                        setterVisibility: SubVisibility.InheritFromParent,
                                        initializerVisibility: SubVisibility.InheritFromParent,
                                        getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(                                ),
                                        setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(                                ),
                                        initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(                                ),
                                        @static: false,
                                        @virtual: false,
                                        @abstract: false,
                                        @protected: false,
                                        @override: false,
                                        @new: false,
                                        visibility: Visibility.Public,
                                        name: @"Dummy",
                                        attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                        typeName: @"System.Type",
                                        isNullable: false,
                                        isValueType: false,
                                        genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(),
                                        defaultValue: null,
                                        explicitInterfaceName: @"",
                                        parentTypeFullName: @""
                                    ),
                                } ),
                                defaultValue: null,
                                explicitInterfaceName: @"",
                                parentTypeFullName: @""
                            ),
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateResultArgument",
                    },
                } ),
                Visibility = Visibility.Internal,
                Name = @"IDelegateResultArgument",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>",
                }),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Delegate",
                        TypeName = @"System.Func<CrossCutting.Common.Results.Result<T>>",
                        GenericTypeArguments = new ObservableCollection<ITypeContainer>(new[]
                        {
                            new Property
                            (
                                hasGetter: true,
                                hasSetter: true,
                                hasInitializer: false,
                                getterVisibility: SubVisibility.InheritFromParent,
                                setterVisibility: SubVisibility.InheritFromParent,
                                initializerVisibility: SubVisibility.InheritFromParent,
                                getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                @static: false,
                                @virtual: false,
                                @abstract: false,
                                @protected: false,
                                @override: false,
                                @new: false,
                                visibility: Visibility.Public,
                                name: @"Dummy",
                                attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                typeName: @"CrossCutting.Common.Results.Result<T>",
                                isNullable: false,
                                isValueType: false,
                                genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(new[]
                                {
                                    new Property
                                    (
                                        hasGetter: true,
                                        hasSetter: true,
                                        hasInitializer: false,
                                        getterVisibility: SubVisibility.InheritFromParent,
                                        setterVisibility: SubVisibility.InheritFromParent,
                                        initializerVisibility: SubVisibility.InheritFromParent,
                                        getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(                                ),
                                        setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(                                ),
                                        initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(                                ),
                                        @static: false,
                                        @virtual: false,
                                        @abstract: false,
                                        @protected: false,
                                        @override: false,
                                        @new: false,
                                        visibility: Visibility.Public,
                                        name: @"Dummy",
                                        attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                        typeName: @"T",
                                        isNullable: false,
                                        isValueType: false,
                                        genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(),
                                        defaultValue: null,
                                        explicitInterfaceName: @"",
                                        parentTypeFullName: @""
                                    ),
                                } ),
                                defaultValue: null,
                                explicitInterfaceName: @"",
                                parentTypeFullName: @""
                            ),
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateResultArgument`1",
                    },
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"ValidationDelegate",
                        TypeName = @"System.Func<CrossCutting.Common.Results.Result<System.Type>>?",
                        IsNullable = true,
                        GenericTypeArguments = new ObservableCollection<ITypeContainer>(new[]
                        {
                            new Property
                            (
                                hasGetter: true,
                                hasSetter: true,
                                hasInitializer: false,
                                getterVisibility: SubVisibility.InheritFromParent,
                                setterVisibility: SubVisibility.InheritFromParent,
                                initializerVisibility: SubVisibility.InheritFromParent,
                                getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(),
                                @static: false,
                                @virtual: false,
                                @abstract: false,
                                @protected: false,
                                @override: false,
                                @new: false,
                                visibility: Visibility.Public,
                                name: @"Dummy",
                                attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                typeName: @"CrossCutting.Common.Results.Result<System.Type>",
                                isNullable: false,
                                isValueType: false,
                                genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(new[]
                                {
                                    new Property
                                    (
                                        hasGetter: true,
                                        hasSetter: true,
                                        hasInitializer: false,
                                        getterVisibility: SubVisibility.InheritFromParent,
                                        setterVisibility: SubVisibility.InheritFromParent,
                                        initializerVisibility: SubVisibility.InheritFromParent,
                                        getterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(                                ),
                                        setterCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(                                ),
                                        initializerCodeStatements: new CrossCutting.Common.ReadOnlyValueCollection<CodeStatementBase>(                                ),
                                        @static: false,
                                        @virtual: false,
                                        @abstract: false,
                                        @protected: false,
                                        @override: false,
                                        @new: false,
                                        visibility: Visibility.Public,
                                        name: @"Dummy",
                                        attributes: new CrossCutting.Common.ReadOnlyValueCollection<Domain.Attribute>(),
                                        typeName: @"System.Type",
                                        isNullable: false,
                                        isValueType: false,
                                        genericTypeArguments: new CrossCutting.Common.ReadOnlyValueCollection<ITypeContainer>(),
                                        defaultValue: null,
                                        explicitInterfaceName: @"",
                                        parentTypeFullName: @""
                                    ),
                                } ),
                                defaultValue: null,
                                explicitInterfaceName: @"",
                                parentTypeFullName: @""
                            ),
                        } ),
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IDelegateResultArgument`1",
                    },
                } ),
                Visibility = Visibility.Internal,
                Name = @"IDelegateResultArgument",
                GenericTypeArguments = new ObservableCollection<string>(new[]
                {
                    @"T",
                }),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                }),
                Visibility = Visibility.Internal,
                Name = @"IEmptyArgument",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument<T>",
                }),
                Visibility = Visibility.Internal,
                Name = @"IEmptyArgument",
                GenericTypeArguments = new ObservableCollection<string>(new[]
                {
                    @"T",
                }),
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                }),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Value",
                        TypeName = @"System.String",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IExpressionArgument",
                    },
                }),
                Visibility = Visibility.Internal,
                Name = @"IExpressionArgument",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.FunctionCallArguments",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.IFunctionCallArgumentBase",
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                }),
                Properties = new ObservableCollection<PropertyBuilder>(new[]
                {
                    new PropertyBuilder
                    {
                        HasGetter = true,
                        Name = @"Function",
                        TypeName = @"CrossCutting.Utilities.Parsers.IFunctionCall",
                        ParentTypeFullName = @"CrossCutting.Utilities.Parsers.FunctionCallArguments.IFunctionArgument",
                    },
                }),
                Visibility = Visibility.Internal,
                Name = @"IFunctionArgument",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.Abstractions",
                Visibility = Visibility.Internal,
                Name = @"IFunctionCallArgument",
            },
            new InterfaceBuilder
            {
                Namespace = @"CrossCutting.Utilities.Parsers.Abstractions",
                Interfaces = new ObservableCollection<string>(new[]
                {
                    @"CrossCutting.Utilities.Parsers.Abstractions.IFunctionCallArgument",
                }),
                Visibility = Visibility.Internal,
                Name = @"IFunctionCallArgument",
                GenericTypeArguments = new ObservableCollection<string>(new[]
                {
                    @"T",
                }),
            },
        } );
#pragma warning restore CA1861 // Avoid constant arrays as arguments
    }
}
