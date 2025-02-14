﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 9.0.2
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
#nullable enable
namespace ClassFramework.Domain.Builders
{
    public abstract partial class CodeStatementBaseBuilder : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        protected CodeStatementBaseBuilder(ClassFramework.Domain.CodeStatementBase source)
        {
        }

        protected CodeStatementBaseBuilder()
        {
            SetDefaultValues();
        }

        public abstract ClassFramework.Domain.CodeStatementBase Build();

        partial void SetDefaultValues();

        public static implicit operator ClassFramework.Domain.CodeStatementBase(CodeStatementBaseBuilder entity)
        {
            return entity.Build();
        }

        protected void HandlePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
    public abstract partial class TypeBaseBuilder : ClassFramework.Domain.Builders.Abstractions.ITypeBuilder, ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder, ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder, ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder, ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder, System.ComponentModel.INotifyPropertyChanged
    {
        private string _namespace;

        private bool _partial;

        private System.Collections.ObjectModel.ObservableCollection<string> _interfaces;

        private System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.FieldBuilder> _fields;

        private System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.PropertyBuilder> _properties;

        private System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.MethodBuilder> _methods;

        private ClassFramework.Domain.Domains.Visibility _visibility;

        private string _name;

        private System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.AttributeBuilder> _attributes;

        private System.Collections.ObjectModel.ObservableCollection<string> _genericTypeArguments;

        private System.Collections.ObjectModel.ObservableCollection<string> _genericTypeArgumentConstraints;

        private System.Collections.ObjectModel.ObservableCollection<string> _suppressWarningCodes;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Namespace
        {
            get
            {
                return _namespace;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_namespace!, value!);
                _namespace = value;
                if (hasChanged) HandlePropertyChanged(nameof(Namespace));
            }
        }

        public bool Partial
        {
            get
            {
                return _partial;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Boolean>.Default.Equals(_partial, value);
                _partial = value;
                if (hasChanged) HandlePropertyChanged(nameof(Partial));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<string> Interfaces
        {
            get
            {
                return _interfaces;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.ObjectModel.ObservableCollection<System.String>>.Default.Equals(_interfaces!, value!);
                _interfaces = value;
                if (hasChanged) HandlePropertyChanged(nameof(Interfaces));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.FieldBuilder> Fields
        {
            get
            {
                return _fields;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Builders.FieldBuilder>>.Default.Equals(_fields!, value!);
                _fields = value;
                if (hasChanged) HandlePropertyChanged(nameof(Fields));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.PropertyBuilder> Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Builders.PropertyBuilder>>.Default.Equals(_properties!, value!);
                _properties = value;
                if (hasChanged) HandlePropertyChanged(nameof(Properties));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.MethodBuilder> Methods
        {
            get
            {
                return _methods;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Builders.MethodBuilder>>.Default.Equals(_methods!, value!);
                _methods = value;
                if (hasChanged) HandlePropertyChanged(nameof(Methods));
            }
        }

        public ClassFramework.Domain.Domains.Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<ClassFramework.Domain.Domains.Visibility>.Default.Equals(_visibility, value);
                _visibility = value;
                if (hasChanged) HandlePropertyChanged(nameof(Visibility));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_name!, value!);
                _name = value;
                if (hasChanged) HandlePropertyChanged(nameof(Name));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.AttributeBuilder> Attributes
        {
            get
            {
                return _attributes;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Builders.AttributeBuilder>>.Default.Equals(_attributes!, value!);
                _attributes = value;
                if (hasChanged) HandlePropertyChanged(nameof(Attributes));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<string> GenericTypeArguments
        {
            get
            {
                return _genericTypeArguments;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.ObjectModel.ObservableCollection<System.String>>.Default.Equals(_genericTypeArguments!, value!);
                _genericTypeArguments = value;
                if (hasChanged) HandlePropertyChanged(nameof(GenericTypeArguments));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.ObjectModel.ObservableCollection<string> GenericTypeArgumentConstraints
        {
            get
            {
                return _genericTypeArgumentConstraints;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.ObjectModel.ObservableCollection<System.String>>.Default.Equals(_genericTypeArgumentConstraints!, value!);
                _genericTypeArgumentConstraints = value;
                if (hasChanged) HandlePropertyChanged(nameof(GenericTypeArgumentConstraints));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Collections.ObjectModel.ObservableCollection<string> SuppressWarningCodes
        {
            get
            {
                return _suppressWarningCodes;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.ObjectModel.ObservableCollection<System.String>>.Default.Equals(_suppressWarningCodes!, value!);
                _suppressWarningCodes = value;
                if (hasChanged) HandlePropertyChanged(nameof(SuppressWarningCodes));
            }
        }

        protected TypeBaseBuilder(ClassFramework.Domain.TypeBase source)
        {
            _interfaces = new System.Collections.ObjectModel.ObservableCollection<string>();
            _fields = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.FieldBuilder>();
            _properties = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.PropertyBuilder>();
            _methods = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.MethodBuilder>();
            _attributes = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.AttributeBuilder>();
            _genericTypeArguments = new System.Collections.ObjectModel.ObservableCollection<string>();
            _genericTypeArgumentConstraints = new System.Collections.ObjectModel.ObservableCollection<string>();
            _suppressWarningCodes = new System.Collections.ObjectModel.ObservableCollection<string>();
            _namespace = source.Namespace;
            _partial = source.Partial;
            foreach (var item in source.Interfaces) _interfaces.Add(item);
            foreach (var item in source.Fields.Select(x => x.ToBuilder())) _fields.Add(item);
            foreach (var item in source.Properties.Select(x => x.ToBuilder())) _properties.Add(item);
            foreach (var item in source.Methods.Select(x => x.ToBuilder())) _methods.Add(item);
            _visibility = source.Visibility;
            _name = source.Name;
            foreach (var item in source.Attributes.Select(x => x.ToBuilder())) _attributes.Add(item);
            foreach (var item in source.GenericTypeArguments) _genericTypeArguments.Add(item);
            foreach (var item in source.GenericTypeArgumentConstraints) _genericTypeArgumentConstraints.Add(item);
            foreach (var item in source.SuppressWarningCodes) _suppressWarningCodes.Add(item);
        }

        protected TypeBaseBuilder()
        {
            _interfaces = new System.Collections.ObjectModel.ObservableCollection<string>();
            _fields = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.FieldBuilder>();
            _properties = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.PropertyBuilder>();
            _methods = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.MethodBuilder>();
            _attributes = new System.Collections.ObjectModel.ObservableCollection<ClassFramework.Domain.Builders.AttributeBuilder>();
            _genericTypeArguments = new System.Collections.ObjectModel.ObservableCollection<string>();
            _genericTypeArgumentConstraints = new System.Collections.ObjectModel.ObservableCollection<string>();
            _suppressWarningCodes = new System.Collections.ObjectModel.ObservableCollection<string>();
            _namespace = string.Empty;
            _name = string.Empty;
            SetDefaultValues();
        }

        public abstract ClassFramework.Domain.TypeBase Build();

        ClassFramework.Domain.Abstractions.IType ClassFramework.Domain.Builders.Abstractions.ITypeBuilder.Build()
        {
            return Build();
        }

        ClassFramework.Domain.Abstractions.IVisibilityContainer ClassFramework.Domain.Builders.Abstractions.IVisibilityContainerBuilder.Build()
        {
            return Build();
        }

        ClassFramework.Domain.Abstractions.INameContainer ClassFramework.Domain.Builders.Abstractions.INameContainerBuilder.Build()
        {
            return Build();
        }

        ClassFramework.Domain.Abstractions.IAttributesContainer ClassFramework.Domain.Builders.Abstractions.IAttributesContainerBuilder.Build()
        {
            return Build();
        }

        ClassFramework.Domain.Abstractions.IGenericTypeArgumentsContainer ClassFramework.Domain.Builders.Abstractions.IGenericTypeArgumentsContainerBuilder.Build()
        {
            return Build();
        }

        ClassFramework.Domain.Abstractions.ISuppressWarningCodesContainer ClassFramework.Domain.Builders.Abstractions.ISuppressWarningCodesContainerBuilder.Build()
        {
            return Build();
        }

        partial void SetDefaultValues();

        public static implicit operator ClassFramework.Domain.TypeBase(TypeBaseBuilder entity)
        {
            return entity.Build();
        }

        protected void HandlePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
#nullable disable
