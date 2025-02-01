﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 9.0.1
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
#nullable enable
namespace ClassFramework.Domain.Builders.Extensions
{
    public static partial class AttributesContainerBuilderExtensions
    {
        public static T AddAttributes<T>(this T instance, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.AttributeBuilder> attributes)
            where T : ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder
        {
            if (attributes is null) throw new System.ArgumentNullException(nameof(attributes));
            return instance.AddAttributes<T>(attributes.ToArray());
        }

        public static T AddAttributes<T>(this T instance, params ClassFramework.Domain.Builders.AttributeBuilder[] attributes)
            where T : ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder
        {
            if (attributes is null) throw new System.ArgumentNullException(nameof(attributes));
            foreach (var item in attributes) instance.Attributes.Add(item);
            return instance;
        }
    }
    public static partial class BaseClassContainerBuilderExtensions
    {
        public static T WithBaseClass<T>(this T instance, string baseClass)
            where T : ClassFramework.Domain.Builders.Abstractions.IBaseClassContainerBuilder
        {
            if (baseClass is null) throw new System.ArgumentNullException(nameof(baseClass));
            instance.BaseClass = baseClass;
            return instance;
        }
    }
    public static partial class CodeStatementsContainerBuilderExtensions
    {
        public static T AddCodeStatements<T>(this T instance, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.CodeStatementBaseBuilder> codeStatements)
            where T : ClassFramework.Domain.Builders.Abstractions.ICodeStatementsContainerBuilder
        {
            if (codeStatements is null) throw new System.ArgumentNullException(nameof(codeStatements));
            return instance.AddCodeStatements<T>(codeStatements.ToArray());
        }

        public static T AddCodeStatements<T>(this T instance, params ClassFramework.Domain.Builders.CodeStatementBaseBuilder[] codeStatements)
            where T : ClassFramework.Domain.Builders.Abstractions.ICodeStatementsContainerBuilder
        {
            if (codeStatements is null) throw new System.ArgumentNullException(nameof(codeStatements));
            foreach (var item in codeStatements) instance.CodeStatements.Add(item);
            return instance;
        }
    }
    public static partial class ConcreteTypeBuilderExtensions
    {
    }
    public static partial class ConstructorsContainerBuilderExtensions
    {
        public static T AddConstructors<T>(this T instance, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.ConstructorBuilder> constructors)
            where T : ClassFramework.Domain.Builders.Abstractions.IConstructorsContainerBuilder
        {
            if (constructors is null) throw new System.ArgumentNullException(nameof(constructors));
            return instance.AddConstructors<T>(constructors.ToArray());
        }

        public static T AddConstructors<T>(this T instance, params ClassFramework.Domain.Builders.ConstructorBuilder[] constructors)
            where T : ClassFramework.Domain.Builders.Abstractions.IConstructorsContainerBuilder
        {
            if (constructors is null) throw new System.ArgumentNullException(nameof(constructors));
            foreach (var item in constructors) instance.Constructors.Add(item);
            return instance;
        }
    }
    public static partial class DefaultValueContainerBuilderExtensions
    {
        public static T WithDefaultValue<T>(this T instance, object? defaultValue)
            where T : ClassFramework.Domain.Builders.Abstractions.IDefaultValueContainerBuilder
        {
            instance.DefaultValue = defaultValue;
            return instance;
        }
    }
    public static partial class EnumsContainerBuilderExtensions
    {
        public static T AddEnums<T>(this T instance, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.EnumerationBuilder> enums)
            where T : ClassFramework.Domain.Builders.Abstractions.IEnumsContainerBuilder
        {
            if (enums is null) throw new System.ArgumentNullException(nameof(enums));
            return instance.AddEnums<T>(enums.ToArray());
        }

        public static T AddEnums<T>(this T instance, params ClassFramework.Domain.Builders.EnumerationBuilder[] enums)
            where T : ClassFramework.Domain.Builders.Abstractions.IEnumsContainerBuilder
        {
            if (enums is null) throw new System.ArgumentNullException(nameof(enums));
            foreach (var item in enums) instance.Enums.Add(item);
            return instance;
        }
    }
    public static partial class ExplicitInterfaceNameContainerBuilderExtensions
    {
        public static T WithExplicitInterfaceName<T>(this T instance, string explicitInterfaceName)
            where T : ClassFramework.Domain.Builders.Abstractions.IExplicitInterfaceNameContainerBuilder
        {
            if (explicitInterfaceName is null) throw new System.ArgumentNullException(nameof(explicitInterfaceName));
            instance.ExplicitInterfaceName = explicitInterfaceName;
            return instance;
        }
    }
    public static partial class GenericTypeArgumentsContainerBuilderExtensions
    {
        public static T AddGenericTypeArguments<T>(this T instance, System.Collections.Generic.IEnumerable<string> genericTypeArguments)
            where T : ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder
        {
            if (genericTypeArguments is null) throw new System.ArgumentNullException(nameof(genericTypeArguments));
            return instance.AddGenericTypeArguments<T>(genericTypeArguments.ToArray());
        }

        public static T AddGenericTypeArguments<T>(this T instance, params string[] genericTypeArguments)
            where T : ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder
        {
            if (genericTypeArguments is null) throw new System.ArgumentNullException(nameof(genericTypeArguments));
            foreach (var item in genericTypeArguments) instance.GenericTypeArguments.Add(item);
            return instance;
        }

        public static T AddGenericTypeArgumentConstraints<T>(this T instance, System.Collections.Generic.IEnumerable<string> genericTypeArgumentConstraints)
            where T : ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder
        {
            if (genericTypeArgumentConstraints is null) throw new System.ArgumentNullException(nameof(genericTypeArgumentConstraints));
            return instance.AddGenericTypeArgumentConstraints<T>(genericTypeArgumentConstraints.ToArray());
        }

        public static T AddGenericTypeArgumentConstraints<T>(this T instance, params string[] genericTypeArgumentConstraints)
            where T : ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder
        {
            if (genericTypeArgumentConstraints is null) throw new System.ArgumentNullException(nameof(genericTypeArgumentConstraints));
            foreach (var item in genericTypeArgumentConstraints) instance.GenericTypeArgumentConstraints.Add(item);
            return instance;
        }
    }
    public static partial class ModifiersContainerBuilderExtensions
    {
        public static T WithStatic<T>(this T instance, bool @static = true)
            where T : ClassFramework.Domain.Builders.Abstractions.IModifiersContainerBuilder
        {
            instance.Static = @static;
            return instance;
        }

        public static T WithVirtual<T>(this T instance, bool @virtual = true)
            where T : ClassFramework.Domain.Builders.Abstractions.IModifiersContainerBuilder
        {
            instance.Virtual = @virtual;
            return instance;
        }

        public static T WithAbstract<T>(this T instance, bool @abstract = true)
            where T : ClassFramework.Domain.Builders.Abstractions.IModifiersContainerBuilder
        {
            instance.Abstract = @abstract;
            return instance;
        }

        public static T WithProtected<T>(this T instance, bool @protected = true)
            where T : ClassFramework.Domain.Builders.Abstractions.IModifiersContainerBuilder
        {
            instance.Protected = @protected;
            return instance;
        }

        public static T WithOverride<T>(this T instance, bool @override = true)
            where T : ClassFramework.Domain.Builders.Abstractions.IModifiersContainerBuilder
        {
            instance.Override = @override;
            return instance;
        }

        public static T WithNew<T>(this T instance, bool @new = true)
            where T : ClassFramework.Domain.Builders.Abstractions.IModifiersContainerBuilder
        {
            instance.New = @new;
            return instance;
        }
    }
    public static partial class NameContainerBuilderExtensions
    {
        public static T WithName<T>(this T instance, string name)
            where T : ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder
        {
            if (name is null) throw new System.ArgumentNullException(nameof(name));
            instance.Name = name;
            return instance;
        }
    }
    public static partial class ParametersContainerBuilderExtensions
    {
        public static T AddParameters<T>(this T instance, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.ParameterBuilder> parameters)
            where T : ClassFramework.Domain.Builders.Abstractions.IParametersContainerBuilder
        {
            if (parameters is null) throw new System.ArgumentNullException(nameof(parameters));
            return instance.AddParameters<T>(parameters.ToArray());
        }

        public static T AddParameters<T>(this T instance, params ClassFramework.Domain.Builders.ParameterBuilder[] parameters)
            where T : ClassFramework.Domain.Builders.Abstractions.IParametersContainerBuilder
        {
            if (parameters is null) throw new System.ArgumentNullException(nameof(parameters));
            foreach (var item in parameters) instance.Parameters.Add(item);
            return instance;
        }
    }
    public static partial class ParentTypeContainerBuilderExtensions
    {
        public static T WithParentTypeFullName<T>(this T instance, string parentTypeFullName)
            where T : ClassFramework.Domain.Builders.Abstractions.IParentTypeContainerBuilder
        {
            if (parentTypeFullName is null) throw new System.ArgumentNullException(nameof(parentTypeFullName));
            instance.ParentTypeFullName = parentTypeFullName;
            return instance;
        }
    }
    public static partial class RecordContainerBuilderExtensions
    {
        public static T WithRecord<T>(this T instance, bool record = true)
            where T : ClassFramework.Domain.Builders.Abstractions.IRecordContainerBuilder
        {
            instance.Record = record;
            return instance;
        }
    }
    public static partial class ReferenceTypeBuilderExtensions
    {
        public static T WithStatic<T>(this T instance, bool @static = true)
            where T : ClassFramework.Domain.Builders.Abstractions.IReferenceTypeBuilder
        {
            instance.Static = @static;
            return instance;
        }

        public static T WithSealed<T>(this T instance, bool @sealed = true)
            where T : ClassFramework.Domain.Builders.Abstractions.IReferenceTypeBuilder
        {
            instance.Sealed = @sealed;
            return instance;
        }

        public static T WithAbstract<T>(this T instance, bool @abstract = true)
            where T : ClassFramework.Domain.Builders.Abstractions.IReferenceTypeBuilder
        {
            instance.Abstract = @abstract;
            return instance;
        }
    }
    public static partial class SubClassesContainerBuilderExtensions
    {
        public static T AddSubClasses<T>(this T instance, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.ITypeBuilder> subClasses)
            where T : ClassFramework.Domain.Builders.Abstractions.ISubClassesContainerBuilder
        {
            if (subClasses is null) throw new System.ArgumentNullException(nameof(subClasses));
            return instance.AddSubClasses<T>(subClasses.ToArray());
        }

        public static T AddSubClasses<T>(this T instance, params ClassFramework.Domain.Builders.ITypeBuilder[] subClasses)
            where T : ClassFramework.Domain.Builders.Abstractions.ISubClassesContainerBuilder
        {
            if (subClasses is null) throw new System.ArgumentNullException(nameof(subClasses));
            foreach (var item in subClasses) instance.SubClasses.Add(item);
            return instance;
        }
    }
    public static partial class SuppressWarningCodesContainerBuilderExtensions
    {
        public static T AddSuppressWarningCodes<T>(this T instance, System.Collections.Generic.IEnumerable<string> suppressWarningCodes)
            where T : ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder
        {
            if (suppressWarningCodes is null) throw new System.ArgumentNullException(nameof(suppressWarningCodes));
            return instance.AddSuppressWarningCodes<T>(suppressWarningCodes.ToArray());
        }

        public static T AddSuppressWarningCodes<T>(this T instance, params string[] suppressWarningCodes)
            where T : ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder
        {
            if (suppressWarningCodes is null) throw new System.ArgumentNullException(nameof(suppressWarningCodes));
            foreach (var item in suppressWarningCodes) instance.SuppressWarningCodes.Add(item);
            return instance;
        }
    }
    public static partial class TypeBuilderExtensions
    {
        public static T AddInterfaces<T>(this T instance, System.Collections.Generic.IEnumerable<string> interfaces)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder
        {
            if (interfaces is null) throw new System.ArgumentNullException(nameof(interfaces));
            return instance.AddInterfaces<T>(interfaces.ToArray());
        }

        public static T AddInterfaces<T>(this T instance, params string[] interfaces)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder
        {
            if (interfaces is null) throw new System.ArgumentNullException(nameof(interfaces));
            foreach (var item in interfaces) instance.Interfaces.Add(item);
            return instance;
        }

        public static T AddFields<T>(this T instance, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.FieldBuilder> fields)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder
        {
            if (fields is null) throw new System.ArgumentNullException(nameof(fields));
            return instance.AddFields<T>(fields.ToArray());
        }

        public static T AddFields<T>(this T instance, params ClassFramework.Domain.Builders.FieldBuilder[] fields)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder
        {
            if (fields is null) throw new System.ArgumentNullException(nameof(fields));
            foreach (var item in fields) instance.Fields.Add(item);
            return instance;
        }

        public static T AddProperties<T>(this T instance, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.PropertyBuilder> properties)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder
        {
            if (properties is null) throw new System.ArgumentNullException(nameof(properties));
            return instance.AddProperties<T>(properties.ToArray());
        }

        public static T AddProperties<T>(this T instance, params ClassFramework.Domain.Builders.PropertyBuilder[] properties)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder
        {
            if (properties is null) throw new System.ArgumentNullException(nameof(properties));
            foreach (var item in properties) instance.Properties.Add(item);
            return instance;
        }

        public static T AddMethods<T>(this T instance, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.MethodBuilder> methods)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder
        {
            if (methods is null) throw new System.ArgumentNullException(nameof(methods));
            return instance.AddMethods<T>(methods.ToArray());
        }

        public static T AddMethods<T>(this T instance, params ClassFramework.Domain.Builders.MethodBuilder[] methods)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder
        {
            if (methods is null) throw new System.ArgumentNullException(nameof(methods));
            foreach (var item in methods) instance.Methods.Add(item);
            return instance;
        }

        public static T WithNamespace<T>(this T instance, string @namespace)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder
        {
            if (@namespace is null) throw new System.ArgumentNullException(nameof(@namespace));
            instance.Namespace = @namespace;
            return instance;
        }

        public static T WithPartial<T>(this T instance, bool partial = true)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder
        {
            instance.Partial = partial;
            return instance;
        }
    }
    public static partial class TypeContainerBuilderExtensions
    {
        public static T AddGenericTypeArguments<T>(this T instance, System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.Abstractions.ITypeContainerBuilder> genericTypeArguments)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeContainerBuilder
        {
            if (genericTypeArguments is null) throw new System.ArgumentNullException(nameof(genericTypeArguments));
            return instance.AddGenericTypeArguments<T>(genericTypeArguments.ToArray());
        }

        public static T AddGenericTypeArguments<T>(this T instance, params ClassFramework.Domain.Builders.Abstractions.ITypeContainerBuilder[] genericTypeArguments)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeContainerBuilder
        {
            if (genericTypeArguments is null) throw new System.ArgumentNullException(nameof(genericTypeArguments));
            foreach (var item in genericTypeArguments) instance.GenericTypeArguments.Add(item);
            return instance;
        }

        public static T WithTypeName<T>(this T instance, string typeName)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeContainerBuilder
        {
            if (typeName is null) throw new System.ArgumentNullException(nameof(typeName));
            instance.TypeName = typeName;
            return instance;
        }

        public static T WithIsNullable<T>(this T instance, bool isNullable = true)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeContainerBuilder
        {
            instance.IsNullable = isNullable;
            return instance;
        }

        public static T WithIsValueType<T>(this T instance, bool isValueType = true)
            where T : ClassFramework.Domain.Builders.Abstractions.ITypeContainerBuilder
        {
            instance.IsValueType = isValueType;
            return instance;
        }
    }
    public static partial class ValueTypeBuilderExtensions
    {
    }
    public static partial class VisibilityContainerBuilderExtensions
    {
        public static T WithVisibility<T>(this T instance, ClassFramework.Domain.Domains.Visibility visibility)
            where T : ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder
        {
            instance.Visibility = visibility;
            return instance;
        }
    }
}
#nullable disable
