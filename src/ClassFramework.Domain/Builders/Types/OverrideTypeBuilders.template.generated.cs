﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 9.0.7
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
#nullable enable
namespace ClassFramework.Domain.Builders.Types
{
    public partial class ClassBuilder : TypeBaseBuilder<ClassBuilder, ClassFramework.Domain.Types.Class>, ClassFramework.Domain.Builders.Abstractions.ITypeBuilder, ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder, ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IReferenceTypeBuilder, ClassFramework.Domain.Builders.Abstractions.IConcreteTypeBuilder, ClassFramework.Domain.Builders.Abstractions.IConstructorsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IRecordContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IBaseClassContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IEnumsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.ISubClassesContainerBuilder
    {
        private bool _static;

        private bool _sealed;

        private bool _abstract;

        private System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.ConstructorBuilder> _constructors;

        private bool _record;

        private string _baseClass;

        private System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.EnumerationBuilder> _enums;

        private System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.TypeBaseBuilder> _subClasses;

        public bool Static
        {
            get
            {
                return _static;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Boolean>.Default.Equals(_static, value);
                _static = value;
                if (hasChanged) HandlePropertyChanged(nameof(Static));
            }
        }

        public bool Sealed
        {
            get
            {
                return _sealed;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Boolean>.Default.Equals(_sealed, value);
                _sealed = value;
                if (hasChanged) HandlePropertyChanged(nameof(Sealed));
            }
        }

        public bool Abstract
        {
            get
            {
                return _abstract;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Boolean>.Default.Equals(_abstract, value);
                _abstract = value;
                if (hasChanged) HandlePropertyChanged(nameof(Abstract));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.ConstructorBuilder> Constructors
        {
            get
            {
                return _constructors;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Builders.ConstructorBuilder>>.Default.Equals(_constructors!, value!);
                _constructors = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Constructors));
            }
        }

        public bool Record
        {
            get
            {
                return _record;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Boolean>.Default.Equals(_record, value);
                _record = value;
                if (hasChanged) HandlePropertyChanged(nameof(Record));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BaseClass
        {
            get
            {
                return _baseClass;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_baseClass!, value!);
                _baseClass = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(BaseClass));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.EnumerationBuilder> Enums
        {
            get
            {
                return _enums;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Builders.EnumerationBuilder>>.Default.Equals(_enums!, value!);
                _enums = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Enums));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.TypeBaseBuilder> SubClasses
        {
            get
            {
                return _subClasses;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Builders.TypeBaseBuilder>>.Default.Equals(_subClasses!, value!);
                _subClasses = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(SubClasses));
            }
        }

        public ClassBuilder(ClassFramework.Domain.Types.Class source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _constructors = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.ConstructorBuilder>();
            _enums = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.EnumerationBuilder>();
            _subClasses = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.TypeBaseBuilder>();
            _static = source.Static;
            _sealed = source.Sealed;
            _abstract = source.Abstract;
            if (source.Constructors is not null) foreach (var item in source.Constructors.Select(x => x.ToBuilder())) _constructors.Add(item);
            _record = source.Record;
            _baseClass = source.BaseClass;
            if (source.Enums is not null) foreach (var item in source.Enums.Select(x => x.ToBuilder())) _enums.Add(item);
            if (source.SubClasses is not null) foreach (var item in source.SubClasses.Select(x => x.ToBuilder())) _subClasses.Add(item);
        }

        public ClassBuilder() : base()
        {
            _constructors = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.ConstructorBuilder>();
            _enums = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.EnumerationBuilder>();
            _subClasses = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.TypeBaseBuilder>();
            _baseClass = string.Empty;
            SetDefaultValues();
        }

        public override ClassFramework.Domain.Types.Class BuildTyped()
        {
            return new ClassFramework.Domain.Types.Class(Namespace, Partial, Interfaces, Fields.Select(x => x.Build()!).ToList().AsReadOnly(), Properties.Select(x => x.Build()!).ToList().AsReadOnly(), Methods.Select(x => x.Build()!).ToList().AsReadOnly(), Visibility, Name, Attributes.Select(x => x.Build()!).ToList().AsReadOnly(), GenericTypeArguments, GenericTypeArgumentConstraints, SuppressWarningCodes, Static, Sealed, Abstract, Constructors.Select(x => x.Build()!).ToList().AsReadOnly(), Record, BaseClass, Enums.Select(x => x.Build()!).ToList().AsReadOnly(), SubClasses.Select(x => x.Build()!).ToList().AsReadOnly());
        }

        ClassFramework.Domain.Abstractions.IType ClassFramework.Domain.Builders.Abstractions.ITypeBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IVisibilityContainer ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.INameContainer ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IAttributesContainer ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IReferenceType ClassFramework.Domain.Builders.Abstractions.IReferenceTypeBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IConcreteType ClassFramework.Domain.Builders.Abstractions.IConcreteTypeBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IConstructorsContainer ClassFramework.Domain.Builders.Abstractions.IConstructorsContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IRecordContainer ClassFramework.Domain.Builders.Abstractions.IRecordContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IBaseClassContainer ClassFramework.Domain.Builders.Abstractions.IBaseClassContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IEnumsContainer ClassFramework.Domain.Builders.Abstractions.IEnumsContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.ISubClassesContainer ClassFramework.Domain.Builders.Abstractions.ISubClassesContainerBuilder.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public static implicit operator ClassFramework.Domain.Types.Class(ClassBuilder entity)
        {
            return entity.BuildTyped();
        }
    }
    public partial class InterfaceBuilder : TypeBaseBuilder<InterfaceBuilder, ClassFramework.Domain.Types.Interface>, ClassFramework.Domain.Builders.Abstractions.ITypeBuilder, ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder, ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder
    {
        public InterfaceBuilder(ClassFramework.Domain.Types.Interface source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
        }

        public InterfaceBuilder() : base()
        {
            SetDefaultValues();
        }

        public override ClassFramework.Domain.Types.Interface BuildTyped()
        {
            return new ClassFramework.Domain.Types.Interface(Namespace, Partial, Interfaces, Fields.Select(x => x.Build()!).ToList().AsReadOnly(), Properties.Select(x => x.Build()!).ToList().AsReadOnly(), Methods.Select(x => x.Build()!).ToList().AsReadOnly(), Visibility, Name, Attributes.Select(x => x.Build()!).ToList().AsReadOnly(), GenericTypeArguments, GenericTypeArgumentConstraints, SuppressWarningCodes);
        }

        ClassFramework.Domain.Abstractions.IType ClassFramework.Domain.Builders.Abstractions.ITypeBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IVisibilityContainer ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.INameContainer ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IAttributesContainer ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public static implicit operator ClassFramework.Domain.Types.Interface(InterfaceBuilder entity)
        {
            return entity.BuildTyped();
        }
    }
    public partial class StructBuilder : TypeBaseBuilder<StructBuilder, ClassFramework.Domain.Types.Struct>, ClassFramework.Domain.Builders.Abstractions.ITypeBuilder, ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder, ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IValueTypeBuilder, ClassFramework.Domain.Builders.Abstractions.IConcreteTypeBuilder, ClassFramework.Domain.Builders.Abstractions.IConstructorsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IRecordContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IBaseClassContainerBuilder
    {
        private System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.ConstructorBuilder> _constructors;

        private bool _record;

        private string _baseClass;

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.ConstructorBuilder> Constructors
        {
            get
            {
                return _constructors;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Builders.ConstructorBuilder>>.Default.Equals(_constructors!, value!);
                _constructors = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Constructors));
            }
        }

        public bool Record
        {
            get
            {
                return _record;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Boolean>.Default.Equals(_record, value);
                _record = value;
                if (hasChanged) HandlePropertyChanged(nameof(Record));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string BaseClass
        {
            get
            {
                return _baseClass;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_baseClass!, value!);
                _baseClass = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(BaseClass));
            }
        }

        public StructBuilder(ClassFramework.Domain.Types.Struct source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _constructors = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.ConstructorBuilder>();
            if (source.Constructors is not null) foreach (var item in source.Constructors.Select(x => x.ToBuilder())) _constructors.Add(item);
            _record = source.Record;
            _baseClass = source.BaseClass;
        }

        public StructBuilder() : base()
        {
            _constructors = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.ConstructorBuilder>();
            _baseClass = string.Empty;
            SetDefaultValues();
        }

        public override ClassFramework.Domain.Types.Struct BuildTyped()
        {
            return new ClassFramework.Domain.Types.Struct(Namespace, Partial, Interfaces, Fields.Select(x => x.Build()!).ToList().AsReadOnly(), Properties.Select(x => x.Build()!).ToList().AsReadOnly(), Methods.Select(x => x.Build()!).ToList().AsReadOnly(), Visibility, Name, Attributes.Select(x => x.Build()!).ToList().AsReadOnly(), GenericTypeArguments, GenericTypeArgumentConstraints, SuppressWarningCodes, Constructors.Select(x => x.Build()!).ToList().AsReadOnly(), Record, BaseClass);
        }

        ClassFramework.Domain.Abstractions.IType ClassFramework.Domain.Builders.Abstractions.ITypeBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IVisibilityContainer ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.INameContainer ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IAttributesContainer ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IValueType ClassFramework.Domain.Builders.Abstractions.IValueTypeBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IConcreteType ClassFramework.Domain.Builders.Abstractions.IConcreteTypeBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IConstructorsContainer ClassFramework.Domain.Builders.Abstractions.IConstructorsContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IRecordContainer ClassFramework.Domain.Builders.Abstractions.IRecordContainerBuilder.Build()
        {
            return BuildTyped();
        }

        ClassFramework.Domain.Abstractions.IBaseClassContainer ClassFramework.Domain.Builders.Abstractions.IBaseClassContainerBuilder.Build()
        {
            return BuildTyped();
        }

        partial void SetDefaultValues();

        public static implicit operator ClassFramework.Domain.Types.Struct(StructBuilder entity)
        {
            return entity.BuildTyped();
        }
    }
}
#nullable disable
