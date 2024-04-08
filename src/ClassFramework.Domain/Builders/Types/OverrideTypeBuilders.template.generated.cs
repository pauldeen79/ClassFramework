﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 8.0.3
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                _static = value;
                HandlePropertyChanged(nameof(Static));
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
                _sealed = value;
                HandlePropertyChanged(nameof(Sealed));
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
                _abstract = value;
                HandlePropertyChanged(nameof(Abstract));
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
                _constructors = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(Constructors));
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
                _record = value;
                HandlePropertyChanged(nameof(Record));
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
                _baseClass = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(BaseClass));
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
                _enums = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(Enums));
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
                _subClasses = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(SubClasses));
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
            if (source.Constructors is not null) foreach (var item in source.Constructors.Select(x => x.ToBuilder()!)) _constructors.Add(item);
            _record = source.Record;
            _baseClass = source.BaseClass;
            if (source.Enums is not null) foreach (var item in source.Enums.Select(x => x.ToBuilder()!)) _enums.Add(item);
            if (source.SubClasses is not null) foreach (var item in source.SubClasses.Select(x => x.ToBuilder()!)) _subClasses.Add(item);
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

        partial void SetDefaultValues();

        public ClassFramework.Domain.Builders.Types.ClassBuilder AddConstructors(System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.ConstructorBuilder> constructors)
        {
            if (constructors is null) throw new System.ArgumentNullException(nameof(constructors));
            return AddConstructors(constructors.ToArray());
        }

        public ClassFramework.Domain.Builders.Types.ClassBuilder AddConstructors(params ClassFramework.Domain.Builders.ConstructorBuilder[] constructors)
        {
            if (constructors is null) throw new System.ArgumentNullException(nameof(constructors));
            foreach (var item in constructors) Constructors.Add(item);
            return this;
        }

        public ClassFramework.Domain.Builders.Types.ClassBuilder AddEnums(System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.EnumerationBuilder> enums)
        {
            if (enums is null) throw new System.ArgumentNullException(nameof(enums));
            return AddEnums(enums.ToArray());
        }

        public ClassFramework.Domain.Builders.Types.ClassBuilder AddEnums(params ClassFramework.Domain.Builders.EnumerationBuilder[] enums)
        {
            if (enums is null) throw new System.ArgumentNullException(nameof(enums));
            foreach (var item in enums) Enums.Add(item);
            return this;
        }

        public ClassFramework.Domain.Builders.Types.ClassBuilder AddSubClasses(System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.TypeBaseBuilder> subClasses)
        {
            if (subClasses is null) throw new System.ArgumentNullException(nameof(subClasses));
            return AddSubClasses(subClasses.ToArray());
        }

        public ClassFramework.Domain.Builders.Types.ClassBuilder AddSubClasses(params ClassFramework.Domain.Builders.TypeBaseBuilder[] subClasses)
        {
            if (subClasses is null) throw new System.ArgumentNullException(nameof(subClasses));
            foreach (var item in subClasses) SubClasses.Add(item);
            return this;
        }

        public ClassFramework.Domain.Builders.Types.ClassBuilder WithStatic(bool @static = true)
        {
            Static = @static;
            return this;
        }

        public ClassFramework.Domain.Builders.Types.ClassBuilder WithSealed(bool @sealed = true)
        {
            Sealed = @sealed;
            return this;
        }

        public ClassFramework.Domain.Builders.Types.ClassBuilder WithAbstract(bool @abstract = true)
        {
            Abstract = @abstract;
            return this;
        }

        public ClassFramework.Domain.Builders.Types.ClassBuilder WithRecord(bool record = true)
        {
            Record = record;
            return this;
        }

        public ClassFramework.Domain.Builders.Types.ClassBuilder WithBaseClass(string baseClass)
        {
            if (baseClass is null) throw new System.ArgumentNullException(nameof(baseClass));
            BaseClass = baseClass;
            return this;
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

        partial void SetDefaultValues();
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
                _constructors = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(Constructors));
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
                _record = value;
                HandlePropertyChanged(nameof(Record));
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
                _baseClass = value ?? throw new System.ArgumentNullException(nameof(value));
                HandlePropertyChanged(nameof(BaseClass));
            }
        }

        public StructBuilder(ClassFramework.Domain.Types.Struct source) : base(source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _constructors = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.ConstructorBuilder>();
            if (source.Constructors is not null) foreach (var item in source.Constructors.Select(x => x.ToBuilder()!)) _constructors.Add(item);
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

        partial void SetDefaultValues();

        public ClassFramework.Domain.Builders.Types.StructBuilder AddConstructors(System.Collections.Generic.IEnumerable<ClassFramework.Domain.Builders.ConstructorBuilder> constructors)
        {
            if (constructors is null) throw new System.ArgumentNullException(nameof(constructors));
            return AddConstructors(constructors.ToArray());
        }

        public ClassFramework.Domain.Builders.Types.StructBuilder AddConstructors(params ClassFramework.Domain.Builders.ConstructorBuilder[] constructors)
        {
            if (constructors is null) throw new System.ArgumentNullException(nameof(constructors));
            foreach (var item in constructors) Constructors.Add(item);
            return this;
        }

        public ClassFramework.Domain.Builders.Types.StructBuilder WithRecord(bool record = true)
        {
            Record = record;
            return this;
        }

        public ClassFramework.Domain.Builders.Types.StructBuilder WithBaseClass(string baseClass)
        {
            if (baseClass is null) throw new System.ArgumentNullException(nameof(baseClass));
            BaseClass = baseClass;
            return this;
        }
    }
}
#nullable disable
