﻿namespace ClassFramework.Pipelines.Extensions;

public static class TypeBaseExtensions
{
    public static bool IsMemberValidForBuilderClass(
        this IType parent,
        IParentTypeContainer parentTypeContainer,
        PipelineSettings settings)
    {
        parentTypeContainer = parentTypeContainer.IsNotNull(nameof(parentTypeContainer));
        settings = settings.IsNotNull(nameof(settings));

        if (!settings.EnableInheritance)
        {
            // If entity inheritance is not enabled, then simply include all members
            return true;
        }

        // If inheritance is enabled, then include the members if it's defined on the parent class (or use the custom instance comparison delegate, when provided)
        return parentTypeContainer.IsDefinedOn(parent, settings.InheritanceComparisonDelegate);
    }

    public static Task<Result<GenericFormattableString>> GetCustomValueForInheritedClassAsync(
        this IType instance,
        bool enableInheritance,
        Func<IBaseClassContainer, Task<Result<GenericFormattableString>>> customValue)
        => instance.GetCustomValueForInheritedClass(enableInheritance, customValue);

    public static async Task<Result<GenericFormattableString>> GetCustomValueForInheritedClass(
        this IType instance,
        bool enableInheritance,
        Func<IBaseClassContainer, Task<Result<GenericFormattableString>>> customValue)
    {
        customValue = customValue.IsNotNull(nameof(customValue));

        if (!enableInheritance)
        {
            // Inheritance is not enabled
            return Result.Success<GenericFormattableString>(string.Empty);
        }

        if (instance is not IBaseClassContainer baseClassContainer)
        {
            // Type cannot have a base class
            return Result.Success<GenericFormattableString>(string.Empty);
        }

        if (string.IsNullOrEmpty(baseClassContainer.BaseClass))
        {
            // Class is not inherited
            return Result.Success<GenericFormattableString>(string.Empty);
        }

        return await customValue(baseClassContainer).ConfigureAwait(false);
    }

    public static IEnumerable<Property> GetBuilderConstructorProperties(
        this IType instance,
        BuilderContext context)
    {
        context = context.IsNotNull(nameof(context));

        if (instance is not IConstructorsContainer constructorsContainer)
        {
            throw new ArgumentException("Cannot get immutable builder constructor properties for type that does not have constructors", nameof(context));
        }

        if (constructorsContainer.HasPublicParameterlessConstructor())
        {
            return instance.Properties.Where(x => x.HasSetter || x.HasInitializer);
        }

        var ctor = constructorsContainer.Constructors.FirstOrDefault(x => x.Visibility == Visibility.Public && x.Parameters.Count > 0);
        if (ctor is null)
        {
            // No public constructor, so we can't add properties to initialization.
            return [];
        }

        if (context.IsBuilderForOverrideEntity && context.Settings.BaseClass is not null)
        {
            // Try to get property from either the base class c'tor or the class c'tor itself
            return ctor
                .Parameters
                .Select(x => instance.Properties.FirstOrDefault(y => y.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase))
                    ?? context.Settings.BaseClass!.Properties.FirstOrDefault(y => y.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
                .Where(x => x is not null);
        }

        return ctor
            .Parameters
            .Select(x => instance.Properties.FirstOrDefault(y => y.Name.Equals(x.Name, StringComparison.OrdinalIgnoreCase)))
            .Where(x => x is not null);
    }

    public static async Task<IEnumerable<Result<FieldBuilder>>> GetBuilderClassFieldsAsync(
        this IType instance,
        PipelineContext<BuilderContext> context,
        IExpressionEvaluator evaluator,
        CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));
        evaluator = evaluator.IsNotNull(nameof(evaluator));

        if (!context.Request.HasBackingFields())
        {
            return Enumerable.Empty<Result<FieldBuilder>>();
        }

        var results = new List<Result<FieldBuilder>>();

        foreach (var property in instance.Properties.Where(x =>
            instance.IsMemberValidForBuilderClass(x, context.Request.Settings)
            && x.HasBackingFieldOnBuilder(context.Request.Settings)))
        {
            var builderArgumentTypeResult = await property.GetBuilderArgumentTypeNameAsync(context.Request, new ParentChildContext<PipelineContext<BuilderContext>, Property>(context, property, context.Request.Settings), context.Request.MapTypeName(property.TypeName, MetadataNames.CustomEntityInterfaceTypeName), evaluator, token).ConfigureAwait(false);
            if (!builderArgumentTypeResult.IsSuccessful())
            {
                results.Add(Result.FromExistingResult<FieldBuilder>(builderArgumentTypeResult));
                break;
            }

            results.Add(Result.Success(new FieldBuilder()
                .WithName($"_{property.Name.ToCamelCase(context.Request.FormatProvider.ToCultureInfo())}")
                .WithTypeName(builderArgumentTypeResult.Value!.ToString().FixCollectionTypeName(context.Request.Settings.BuilderNewCollectionTypeName).FixNullableTypeName(property))
                .WithIsNullable(property.IsNullable)
                .WithIsValueType(property.IsValueType)
                .AddGenericTypeArguments(property.GenericTypeArguments.Select(x => x.ToBuilder()))));
        }

        return results;
    }

    public static async Task<string> GetEntityBaseClassAsync(this IType instance, bool enableInheritance, IType? baseClass)
        => enableInheritance
        && baseClass is not null
            ? baseClass.GetFullName()
            : (await instance.GetCustomValueForInheritedClassAsync(enableInheritance, cls => Task.FromResult(Result.Success<GenericFormattableString>(cls.BaseClass))).ConfigureAwait(false)).Value!; // we're always returning Success here, so we can shortcut the validation of the result by getting .Value

    public static string WithoutInterfacePrefix(this IType instance)
        => instance is Domain.Types.Interface
            && instance.Name.StartsWith("I")
            && instance.Name.Length >= 2
            && instance.Name.Substring(1, 1).Equals(instance.Name.Substring(1, 1).ToUpperInvariant(), StringComparison.Ordinal)
                ? instance.Name.Substring(1)
                : instance.Name;
}
