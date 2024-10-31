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
        CollectionCopyStatementFormatString = "foreach (var item in {NameCamelCsharpFriendlyName}) {InstancePrefix}{Name}.Add(item);";
        NonCollectionInitializationStatementFormatString = "source.[SourceExpression]"; // note that we are not prefixing {NullCheck.Source.Argument}, because we can simply always copy the value, regardless if it's null :)
        BuilderExtensionsNamespaceFormatString = "{Namespace}.Builders.Extensions";
        BuilderExtensionsNameFormatString = "{Class.NameNoInterfacePrefix}BuilderExtensions";
        BuilderExtensionsCollectionCopyStatementFormatString = "foreach (var item in {NameCamelCsharpFriendlyName}) {InstancePrefix}{Name}.Add(item);";
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
        UseDefaultValueAttributeValuesForBuilderInitialization = true;
        AttributeInitializers.Add(x => GetInitializer<StringLengthAttribute>(x, stringLengthAttribute =>
            new AttributeBuilder().WithName(stringLengthAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(stringLengthAttribute.MaximumLength))
                .AddParameters(CreateConditional(() => stringLengthAttribute.MinimumLength > 0, () => new AttributeParameterBuilder().WithValue(stringLengthAttribute.MinimumLength).WithName(nameof(stringLengthAttribute.MinimumLength))))
                .AddParameters(ErrorMessage(stringLengthAttribute))
                .Build()));
        AttributeInitializers.Add(x => GetInitializer<RangeAttribute>(x, rangeAttribute =>
            new AttributeBuilder().WithName(rangeAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(rangeAttribute.Minimum))
                .AddParameters(new AttributeParameterBuilder().WithValue(rangeAttribute.Maximum))
                .AddParameters(ErrorMessage(rangeAttribute))
                .Build()));
        AttributeInitializers.Add(x => GetInitializer<MinLengthAttribute>(x, minLengthAttribute =>
            new AttributeBuilder().WithName(minLengthAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(minLengthAttribute.Length))
                .AddParameters(ErrorMessage(minLengthAttribute))
                .Build()));
        AttributeInitializers.Add(x => GetInitializer<MaxLengthAttribute>(x, maxLengthAttribute =>
            new AttributeBuilder().WithName(maxLengthAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(maxLengthAttribute.Length))
                .AddParameters(ErrorMessage(maxLengthAttribute))
                .Build()));
        AttributeInitializers.Add(x => GetInitializer<RegularExpressionAttribute>(x, regularExpressionAttribute =>
            new AttributeBuilder().WithName(regularExpressionAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(regularExpressionAttribute.Pattern))
                .AddParameters(CreateConditional(() => regularExpressionAttribute.MatchTimeoutInMilliseconds != 2000, () => new AttributeParameterBuilder().WithValue(regularExpressionAttribute.MatchTimeoutInMilliseconds).WithName(nameof(RegularExpressionAttribute.MatchTimeoutInMilliseconds))))
                .AddParameters(ErrorMessage(regularExpressionAttribute))
                .Build()));
        AttributeInitializers.Add(x => GetInitializer<RequiredAttribute>(x, requiredAttribute =>
            new AttributeBuilder().WithName(requiredAttribute.GetType())
                .AddParameters(CreateConditional(() => requiredAttribute.AllowEmptyStrings, () => new AttributeParameterBuilder().WithValue(requiredAttribute.AllowEmptyStrings).WithName(nameof(RequiredAttribute.AllowEmptyStrings))))
                .AddParameters(ErrorMessage(requiredAttribute))
                .Build()));
        AttributeInitializers.Add(x => GetInitializer<MinCountAttribute>(x, minCountAttribute =>
            new AttributeBuilder().WithName(minCountAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(minCountAttribute.Count))
                .AddParameters(ErrorMessage(minCountAttribute))
                .Build()));
        AttributeInitializers.Add(x => GetInitializer<MaxCountAttribute>(x, maxCountAttribute =>
            new AttributeBuilder().WithName(maxCountAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(maxCountAttribute.Count))
                .AddParameters(ErrorMessage(maxCountAttribute))
                .Build()));
        AttributeInitializers.Add(x => GetInitializer<CountAttribute>(x, countAttribute =>
            new AttributeBuilder().WithName(countAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(countAttribute.MinimumCount))
                .AddParameters(new AttributeParameterBuilder().WithValue(countAttribute.MaximumCount))
                .AddParameters(ErrorMessage(countAttribute))
                .Build()));
        AttributeInitializers.Add(x => GetInitializer<ValidationAttribute>(x, validationAttribute => Array.Exists(x.GetType().GetConstructors(), y => y.GetParameters().Length == 0)
            ? new AttributeBuilder().WithName(validationAttribute.GetType())
                .AddParameters(ErrorMessage(validationAttribute)).
                Build()
            : null));
        AttributeInitializers.Add(x => GetInitializer<DefaultValueAttribute>(x, defaultValueAttribute =>
            new AttributeBuilder().WithName(defaultValueAttribute.GetType())
                .AddParameters(new AttributeParameterBuilder().WithValue(defaultValueAttribute.Value))
                .Build()));
        
        // Fallback as latest
        AttributeInitializers.Add(x =>
        {
            var ctor = GetConstructor(x.GetType());
            if (ctor is null)
            {
                return null;
            }

            return new AttributeBuilder()
                .WithName(x.GetType())
                .AddParameters(ctor.GetParameters().Select(y => new AttributeParameterBuilder().WithName(y.Name).WithValue(GetValue(x, y.Name))))
                .Build();
        });
    }

    private object? GetValue(System.Attribute x, string name)
    {
        var type = x.GetType();
        var prop = type.GetProperty(name.ToPascalCase(CultureInfo.InvariantCulture));
        if (prop is not null)
        {
            return prop.GetValue(x);
        }

        return null;
    }

    private static ConstructorInfo? GetConstructor(Type type)
    {
        // If multiple constructors are present, then take the one with the most arguments
        return type.GetConstructors().OrderByDescending(x => x.GetParameters().Length).FirstOrDefault();
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

    private static Domain.Attribute? GetInitializer<T>(System.Attribute sourceAttribute, Func<T, Domain.Attribute?> initializer)
        => sourceAttribute is T typed
            ? initializer(typed)
            : null;
}
