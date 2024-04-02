namespace ClassFramework.Pipelines;

public static class MetadataNames
{
    /// <summary>
    /// Metadata name for defining a custom typename for argument in a builder. Note that the value will be converted to string.
    /// </summary>
    public const string CustomBuilderArgumentType = "ClassFramework.Builder.ArgumentType";

    /// <summary>
    /// Metadata name for defining a custom default value in builder c'tor.
    /// </summary>
    public const string CustomBuilderDefaultValue = "ClassFramework.Builder.Ctor.CustomDefaultValue";

    /// <summary>
    /// Metadata name for defining a custom initialization in the constructor of the builder. Note that the value will be converted to string.
    /// </summary>
    public const string CustomBuilderConstructorInitializeExpression = "ClassFramework.Builder.ConstructorInitializeExpression";

    /// <summary>
    /// Metadata name for defining a custom source expression (e.g. [Name] or [Name].ToBuilder()). Note that the value will be converted to string.
    /// </summary>
    public const string CustomBuilderSourceExpression = "ClassFramework.Builder.SourceExpression";

    /// <summary>
    /// Metadata name for defining a custom parameter expression in the builder. Note that the value will be converted to string.
    /// </summary>
    public const string CustomBuilderMethodParameterExpression = "ClassFramework.Builder.Ctor.MethodParameterExpression";

    /// <summary>
    /// Metadata name for defining default value for a property on a fluent (With/Set) method in a builder. Note that the value is of type System.Object, and will be rendered using CsharpFormat in code generation.
    /// </summary>
    public const string CustomBuilderWithDefaultPropertyValue = "ClassFramework.Builder.Property.DefaultValue";

    /// <summary>
    /// Metadata name for defining a custom initialization on a fluent (With/Set) method in a builder. Note that the value will be converted to string.
    /// </summary>
    public const string CustomBuilderWithExpression = "ClassFramework.Builder.WithExpression";

    /// <summary>
    /// Metadata name for defining a custom argument null check on a fluent (With/Set) method in a builder. Note that the value will be converted to string.
    /// </summary>
    public const string CustomBuilderArgumentNullCheckExpression = "ClassFramework.Builder.ArgumentNullCheck";

    /// <summary>
    /// Metadata name for defining a custom initialization on a fluent (Add) method in a builder. Note that the value will be converted to string.
    /// </summary>
    public const string CustomBuilderAddExpression = "ClassFramework.Builder.AddExpression";

    /// <summary>
    /// Metadata name for defining a custom entity instanciation on a builder. Note that the value will be converted to string. Example: new MyType { Property1 = Property1 }
    /// </summary>
    public const string CustomBuilderEntityInstanciation = "ClassFramework.Builder.EntityInstanciation";

    /// <summary>
    /// Metadata name for defining a custom builder namespace. Note that the value will be converted to string.
    /// </summary>
    public const string CustomBuilderNamespace = "ClassFramework.Builder.Namespace";

    /// <summary>
    /// Metadata name for defining a custom builder name. Note that the value will be converted to string.
    /// </summary>
    public const string CustomBuilderName = "ClassFramework.Builder.Name";

    /// <summary>
    /// Metadata name for defining a custom builder interface namespace. Note that the value will be converted to string.
    /// </summary>
    public const string CustomBuilderInterfaceNamespace = "ClassFramework.Builder.Interface.Namespace";

    /// <summary>
    /// Metadata name for defining a custom builder interface name. Note that the value will be converted to string.
    /// </summary>
    public const string CustomBuilderInterfaceName = "ClassFramework.Builder.Interface.Name";

    /// <summary>
    /// Metadata name for defining a custom builder parent type namespace. Note that the value will be converted to string.
    /// </summary>
    public const string CustomBuilderParentTypeNamespace = "ClassFramework.Builder.ParentType.Namespace";

    /// <summary>
    /// Metadata name for defining a custom builder parent type name. Note that the value will be converted to string.
    /// </summary>
    public const string CustomBuilderParentTypeName = "ClassFramework.Builder.ParentType.Name";

    /// <summary>
    /// Metadata name for defining a custom entity namespace. Note that the value will be converted to string.
    /// </summary>
    public const string CustomEntityNamespace = "ClassFramework.Entity.Namespace";

    /// <summary>
    /// Metadata name for defining if the entity should be checked for null. Value needs to be a boolean.
    /// </summary>
    public const string EntityNullCheck = "ClassFramework.Entity.NullCheck";

    /// <summary>
    /// Metadata name for defining custom initialization of collections. Value will be converted to string.
    /// </summary>
    public const string CustomCollectionInitialization = "ClassFramework.Entity.Collection.Type.For.Initialization";

    /// <summary>
    /// Metadata name for defining custom parameters for copy constructors. Value needs to be of type Parameter.
    /// </summary>
    public const string CustomBuilderCopyConstructorParameter = "ClassFramework.Builder.CopyConstructor.Parameter";

    /// <summary>
    /// Metadata name for defining custom interface typename on entities
    /// </summary>
    public const string CustomEntityInterfaceTypeName = "ClassFramework.Entity.Interface.TypeName";

    /// <summary>
    /// Metadata name for fixing typename when code generation fails.
    /// </summary>
    public const string CustomTypeName = "ClassFramework.CodeGeneration.Quirk.TypeName.Custom";
}
