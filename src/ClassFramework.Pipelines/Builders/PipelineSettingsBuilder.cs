namespace ClassFramework.Pipelines.Builders;

public partial class PipelineSettingsBuilder
{
    partial void SetDefaultValues()
    {
        AddCopyConstructor = true;
        SetDefaultValuesInEntityConstructor = true;
        SetMethodNameFormatString = "With{Name}";
        AddMethodNameFormatString = "Add{Name}";
        BuilderNamespaceFormatString = "{Namespace}.Builders";
        BuilderNameFormatString = "{Class.Name}Builder";
        BuildMethodName = "Build";
        BuildTypedMethodName = "BuildTyped";
        SetDefaultValuesMethodName = "SetDefaultValues";
        BuilderNewCollectionTypeName = "System.Collections.Generic.IReadOnlyCollection";
        CollectionInitializationStatementFormatString = "{NullCheck.Source.Argument}foreach (var item in source.[SourceExpression]) {BuilderMemberName}.Add(item)";
        CollectionCopyStatementFormatString = "foreach (var item in {NamePascalCsharpFriendlyName}) {InstancePrefix}{Name}.Add(item);";
        NonCollectionInitializationStatementFormatString = "source.[SourceExpression]"; // note that we are not prefixing {NullCheck.Source.Argument}, because we can simply always copy the value, regardless if it's null :)
        BuilderExtensionsNamespaceFormatString = "{Namespace}.Builders.Extensions";
        BuilderExtensionsNameFormatString = "{Class.NameNoInterfacePrefix}BuilderExtensions";
        BuilderExtensionsCollectionCopyStatementFormatString = "foreach (var item in {NamePascalCsharpFriendlyName}) {InstancePrefix}{Name}.Add(item);";
        EntityNamespaceFormatString = "{Namespace}";
        EntityNameFormatString = "{Class.Name}";
        ToBuilderFormatString = "ToBuilder";
        ToTypedBuilderFormatString = "ToTypedBuilder";
        EntityNewCollectionTypeName = "System.Collections.Generic.List";
        NamespaceFormatString = "{Namespace}";
        NameFormatString = "{Class.Name}";
        UseBaseClassFromSourceModel = true;
        CreateAsPartial = true;
        CreateConstructors = true;
        AttributeInitializeDelegate = DefaultInitializer;
    }

    private static Domain.Attribute DefaultInitializer(System.Attribute sourceAttribute)
        => sourceAttribute switch
        {
            StringLengthAttribute stringLengthAttribute =>
                new AttributeBuilder().WithName(stringLengthAttribute.GetType())
                    .AddParameters(new AttributeParameterBuilder().WithValue(stringLengthAttribute.MaximumLength))
                    .AddParameters(CreateConditional(() => stringLengthAttribute.MinimumLength > 0, () => new AttributeParameterBuilder().WithValue(stringLengthAttribute.MinimumLength).WithName(nameof(stringLengthAttribute.MinimumLength))))
                    .AddParameters(ErrorMessage(stringLengthAttribute)).Build(),
            RangeAttribute rangeAttribute =>
                new AttributeBuilder().WithName(rangeAttribute.GetType())
                    .AddParameters(new AttributeParameterBuilder().WithValue(rangeAttribute.Minimum))
                    .AddParameters(new AttributeParameterBuilder().WithValue(rangeAttribute.Maximum))
                    .AddParameters(ErrorMessage(rangeAttribute)).Build(),
            MinLengthAttribute minLengthAttribute =>
                new AttributeBuilder().WithName(minLengthAttribute.GetType())
                    .AddParameters(new AttributeParameterBuilder().WithValue(minLengthAttribute.Length))
                    .AddParameters(ErrorMessage(minLengthAttribute)).Build(),
            MaxLengthAttribute maxLengthAttribute =>
                new AttributeBuilder().WithName(maxLengthAttribute.GetType())
                    .AddParameters(new AttributeParameterBuilder().WithValue(maxLengthAttribute.Length))
                    .AddParameters(ErrorMessage(maxLengthAttribute)).Build(),
            RegularExpressionAttribute regularExpressionAttribute =>
                new AttributeBuilder().WithName(regularExpressionAttribute.GetType())
                    .AddParameters(new AttributeParameterBuilder().WithValue(regularExpressionAttribute.Pattern))
                    .AddParameters(CreateConditional(() => regularExpressionAttribute.MatchTimeoutInMilliseconds != 2000, () => new AttributeParameterBuilder().WithValue(regularExpressionAttribute.MatchTimeoutInMilliseconds).WithName(nameof(RegularExpressionAttribute.MatchTimeoutInMilliseconds))))
                    .AddParameters(ErrorMessage(regularExpressionAttribute)).Build(),
            RequiredAttribute requiredAttribute =>
                new AttributeBuilder().WithName(requiredAttribute.GetType())
                    .AddParameters(CreateConditional(() => requiredAttribute.AllowEmptyStrings, () => new AttributeParameterBuilder().WithValue(requiredAttribute.AllowEmptyStrings).WithName(nameof(RequiredAttribute.AllowEmptyStrings))))
                    .AddParameters(ErrorMessage(requiredAttribute)).Build(),
            ValidationAttribute validationAttribute =>
                new AttributeBuilder().WithName(validationAttribute.GetType())
                    .AddParameters(ErrorMessage(validationAttribute)).Build(),
            _ => new AttributeBuilder().WithName(sourceAttribute.GetType()).Build()
        };

    private static IEnumerable<AttributeParameterBuilder> ErrorMessage(ValidationAttribute validationAttribute)
        => CreateConditional(() => !string.IsNullOrEmpty(validationAttribute.ErrorMessage), () => new AttributeParameterBuilder().WithValue(validationAttribute.ErrorMessage).WithName(nameof(ValidationAttribute.ErrorMessage)));

    private static IEnumerable<AttributeParameterBuilder> CreateConditional(Func<bool> condition, Func<AttributeParameterBuilder> result)
    {
        if (condition.Invoke())
        {
            yield return result.Invoke();
        }
    }
}
