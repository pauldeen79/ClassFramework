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
        AttributeInitializers.Add(new AttributeInitializerBuilder().WithResult(x => x is StringLengthAttribute stringLengthAttribute
            ? new AttributeBuilder().WithName(stringLengthAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(stringLengthAttribute.MaximumLength))
                .AddParameters(CreateConditional(() => stringLengthAttribute.MinimumLength > 0, () => new AttributeParameterBuilder().WithValue(stringLengthAttribute.MinimumLength).WithName(nameof(stringLengthAttribute.MinimumLength))))
                .AddParameters(ErrorMessage(stringLengthAttribute))
                .Build()
            : null));
        AttributeInitializers.Add(new AttributeInitializerBuilder().WithResult(x => x is RangeAttribute rangeAttribute
            ? new AttributeBuilder().WithName(rangeAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(rangeAttribute.Minimum))
                .AddParameters(new AttributeParameterBuilder().WithValue(rangeAttribute.Maximum))
                .AddParameters(ErrorMessage(rangeAttribute))
                .Build()
            : null));
        AttributeInitializers.Add(new AttributeInitializerBuilder().WithResult(x => x is MinLengthAttribute minLengthAttribute
            ? new AttributeBuilder().WithName(minLengthAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(minLengthAttribute.Length))
                .AddParameters(ErrorMessage(minLengthAttribute))
                .Build()
            : null));
        AttributeInitializers.Add(new AttributeInitializerBuilder().WithResult(x => x is MaxLengthAttribute maxLengthAttribute
            ? new AttributeBuilder().WithName(maxLengthAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(maxLengthAttribute.Length))
                .AddParameters(ErrorMessage(maxLengthAttribute))
                .Build()
            : null));
        AttributeInitializers.Add(new AttributeInitializerBuilder().WithResult(x => x is RegularExpressionAttribute regularExpressionAttribute
            ? new AttributeBuilder().WithName(regularExpressionAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(regularExpressionAttribute.Pattern))
                .AddParameters(CreateConditional(() => regularExpressionAttribute.MatchTimeoutInMilliseconds != 2000, () => new AttributeParameterBuilder().WithValue(regularExpressionAttribute.MatchTimeoutInMilliseconds).WithName(nameof(RegularExpressionAttribute.MatchTimeoutInMilliseconds))))
                .AddParameters(ErrorMessage(regularExpressionAttribute))
                .Build()
            : null));
        AttributeInitializers.Add(new AttributeInitializerBuilder().WithResult(x => x is RequiredAttribute requiredAttribute
            ? new AttributeBuilder().WithName(requiredAttribute.GetType())
                .AddParameters(CreateConditional(() => requiredAttribute.AllowEmptyStrings, () => new AttributeParameterBuilder().WithValue(requiredAttribute.AllowEmptyStrings).WithName(nameof(RequiredAttribute.AllowEmptyStrings))))
                .AddParameters(ErrorMessage(requiredAttribute))
                .Build()
            : null));
        AttributeInitializers.Add(new AttributeInitializerBuilder().WithResult(x => x is MinCountAttribute minCountAttribute
            ? new AttributeBuilder().WithName(minCountAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(minCountAttribute.Count))
                .AddParameters(ErrorMessage(minCountAttribute))
                .Build()
            : null));
        AttributeInitializers.Add(new AttributeInitializerBuilder().WithResult(x => x is MaxCountAttribute maxCountAttribute
            ? new AttributeBuilder().WithName(maxCountAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(maxCountAttribute.Count))
                .AddParameters(ErrorMessage(maxCountAttribute))
                .Build()
            : null));
        AttributeInitializers.Add(new AttributeInitializerBuilder().WithResult(x => x is CountAttribute countAttribute
            ? new AttributeBuilder().WithName(countAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(countAttribute.MinimumCount))
                .AddParameters(new AttributeParameterBuilder().WithValue(countAttribute.MaximumCount))
                .AddParameters(ErrorMessage(countAttribute))
                .Build()
            : null));
        AttributeInitializers.Add(new AttributeInitializerBuilder().WithResult(x => x is ValidationAttribute validationAttribute && Array.Exists(x.GetType().GetConstructors(), y => y.GetParameters().Length == 0)
            ? new AttributeBuilder().WithName(validationAttribute.GetType())
                .AddParameters(ErrorMessage(validationAttribute)).
                Build()
            : null));
        AttributeInitializers.Add(new AttributeInitializerBuilder().WithResult(x => x is DefaultValueAttribute defaultValueAttribute
            ? new AttributeBuilder().WithName(defaultValueAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(defaultValueAttribute.Value))
                .Build()
            : null));
        // Fallback as latest
        AttributeInitializers.Add(new AttributeInitializerBuilder().WithResult(x => Array.Exists(x.GetType().GetConstructors(), y => y.GetParameters().Length == 0)
            ? new AttributeBuilder().WithName(x.GetType()).Build()
            : null));
    }

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
