﻿namespace ClassFramework.Pipelines.Builders;

public partial class PipelineSettingsBuilder
{
    partial void SetDefaultValues()
    {
        AddCopyConstructor = true;
        SetDefaultValuesInEntityConstructor = true;
        SetMethodNameFormatString = "With{property.Name}";
        AddMethodNameFormatString = "Add{property.Name}";
        BuilderNamespaceFormatString = "{class.Namespace}.Builders";
        BuilderNameFormatString = "{class.Name}Builder";
        BuildMethodName = "Build";
        BuildTypedMethodName = "BuildTyped";
        SetDefaultValuesMethodName = "SetDefaultValues";
        BuilderNewCollectionTypeName = typeof(IReadOnlyCollection<>).WithoutGenerics();
        CollectionInitializationStatementFormatString = "{SourceArgumentNullCheck()}foreach (var item in source.[SourceExpression]) {property.BuilderMemberName}.Add({property.BuilderFuncPrefix}item{property.BuilderFuncSuffix})";
        CollectionCopyStatementFormatString = "foreach (var item in {CsharpFriendlyName(property.Name.ToCamelCase())}) {InstancePrefix()}{property.Name}.Add(item);";
        NonLazyCollectionCopyStatementFormatString = "foreach (var item in {CsharpFriendlyName(property.Name.ToCamelCase())}) {InstancePrefix()}{property.Name}.Add(() => item);";
        NonCollectionInitializationStatementFormatString = "{property.BuilderFuncPrefix}source.[SourceExpression]{property.BuilderFuncSuffix}";
        BuilderExtensionsNamespaceFormatString = "{class.Namespace}.Builders.Extensions";
        BuilderExtensionsNameFormatString = "{NoInterfacePrefix(class.Name)}BuilderExtensions";
        BuilderExtensionsCollectionCopyStatementFormatString = "foreach (var item in {CsharpFriendlyName(property.Name.ToCamelCase())}) {InstancePrefix()}{property.Name}.Add(item);";
        NonLazyBuilderExtensionsCollectionCopyStatementFormatString = "foreach (var item in {CsharpFriendlyName(property.Name.ToCamelCase())}) {InstancePrefix()}{property.Name}.Add(() => item);";
        EntityNamespaceFormatString = "{class.Namespace}";
        EntityNameFormatString = "{class.Name}";
        ToBuilderFormatString = "ToBuilder";
        ToTypedBuilderFormatString = "ToTypedBuilder";
        EntityNewCollectionTypeName = typeof(List<>).WithoutGenerics();
        NamespaceFormatString = "{class.Namespace}";
        NameFormatString = "{class.Name}";
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
                .AddParameters(ErrorMessage(validationAttribute))
                .Build()
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
                .AddParameters(ctor.GetParameters().Select(y => new AttributeParameterBuilder().WithValue(GetValue(x, y.Name))))
                .Build();
        });
    }

    private static object? GetValue(System.Attribute sourceAttribute, string name)
        => sourceAttribute.GetType().GetProperty(name.ToPascalCase(CultureInfo.InvariantCulture))?.GetValue(sourceAttribute);

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
