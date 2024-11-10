﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 8.0.10
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
#nullable enable
namespace ClassFramework.TemplateFramework.Builders
{
    public partial class CsharpClassGeneratorSettingsBuilder : System.ComponentModel.INotifyPropertyChanged
    {
        private bool _recurseOnDeleteGeneratedFiles;

        private string _lastGeneratedFilesFilename;

        private System.Text.Encoding _encoding;

        private string _path;

        private System.Globalization.CultureInfo _cultureInfo;

        private bool _generateMultipleFiles;

        private bool _skipWhenFileExists;

        private bool _createCodeGenerationHeader;

        private string _environmentVersion;

        private string _filenameSuffix;

        private bool _enableNullableContext;

        private bool _enableNullablePragmas;

        private bool _enableGlobalUsings;

        private System.Collections.ObjectModel.ObservableCollection<string> _customUsings;

        private System.Collections.ObjectModel.ObservableCollection<string> _namespacesToAbbreviate;

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        public bool RecurseOnDeleteGeneratedFiles
        {
            get
            {
                return _recurseOnDeleteGeneratedFiles;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Boolean>.Default.Equals(_recurseOnDeleteGeneratedFiles, value);
                _recurseOnDeleteGeneratedFiles = value;
                if (hasChanged) HandlePropertyChanged(nameof(RecurseOnDeleteGeneratedFiles));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string LastGeneratedFilesFilename
        {
            get
            {
                return _lastGeneratedFilesFilename;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_lastGeneratedFilesFilename!, value!);
                _lastGeneratedFilesFilename = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(LastGeneratedFilesFilename));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Text.Encoding Encoding
        {
            get
            {
                return _encoding;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Text.Encoding>.Default.Equals(_encoding!, value!);
                _encoding = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Encoding));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_path!, value!);
                _path = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(Path));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Globalization.CultureInfo CultureInfo
        {
            get
            {
                return _cultureInfo;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Globalization.CultureInfo>.Default.Equals(_cultureInfo!, value!);
                _cultureInfo = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(CultureInfo));
            }
        }

        public bool GenerateMultipleFiles
        {
            get
            {
                return _generateMultipleFiles;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Boolean>.Default.Equals(_generateMultipleFiles, value);
                _generateMultipleFiles = value;
                if (hasChanged) HandlePropertyChanged(nameof(GenerateMultipleFiles));
            }
        }

        public bool SkipWhenFileExists
        {
            get
            {
                return _skipWhenFileExists;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Boolean>.Default.Equals(_skipWhenFileExists, value);
                _skipWhenFileExists = value;
                if (hasChanged) HandlePropertyChanged(nameof(SkipWhenFileExists));
            }
        }

        public bool CreateCodeGenerationHeader
        {
            get
            {
                return _createCodeGenerationHeader;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Boolean>.Default.Equals(_createCodeGenerationHeader, value);
                _createCodeGenerationHeader = value;
                if (hasChanged) HandlePropertyChanged(nameof(CreateCodeGenerationHeader));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string EnvironmentVersion
        {
            get
            {
                return _environmentVersion;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_environmentVersion!, value!);
                _environmentVersion = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(EnvironmentVersion));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string FilenameSuffix
        {
            get
            {
                return _filenameSuffix;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.String>.Default.Equals(_filenameSuffix!, value!);
                _filenameSuffix = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(FilenameSuffix));
            }
        }

        public bool EnableNullableContext
        {
            get
            {
                return _enableNullableContext;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Boolean>.Default.Equals(_enableNullableContext, value);
                _enableNullableContext = value;
                if (hasChanged) HandlePropertyChanged(nameof(EnableNullableContext));
            }
        }

        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool EnableNullablePragmas
        {
            get
            {
                return _enableNullablePragmas;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Boolean>.Default.Equals(_enableNullablePragmas, value);
                _enableNullablePragmas = value;
                if (hasChanged) HandlePropertyChanged(nameof(EnableNullablePragmas));
            }
        }

        public bool EnableGlobalUsings
        {
            get
            {
                return _enableGlobalUsings;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Boolean>.Default.Equals(_enableGlobalUsings, value);
                _enableGlobalUsings = value;
                if (hasChanged) HandlePropertyChanged(nameof(EnableGlobalUsings));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Collections.ObjectModel.ObservableCollection<string> CustomUsings
        {
            get
            {
                return _customUsings;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.ObjectModel.ObservableCollection<System.String>>.Default.Equals(_customUsings!, value!);
                _customUsings = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(CustomUsings));
            }
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Collections.ObjectModel.ObservableCollection<string> NamespacesToAbbreviate
        {
            get
            {
                return _namespacesToAbbreviate;
            }
            set
            {
                bool hasChanged = !System.Collections.Generic.EqualityComparer<System.Collections.ObjectModel.ObservableCollection<System.String>>.Default.Equals(_namespacesToAbbreviate!, value!);
                _namespacesToAbbreviate = value ?? throw new System.ArgumentNullException(nameof(value));
                if (hasChanged) HandlePropertyChanged(nameof(NamespacesToAbbreviate));
            }
        }

        public CsharpClassGeneratorSettingsBuilder(ClassFramework.TemplateFramework.CsharpClassGeneratorSettings source)
        {
            if (source is null) throw new System.ArgumentNullException(nameof(source));
            _customUsings = new System.Collections.ObjectModel.ObservableCollection<string>();
            _namespacesToAbbreviate = new System.Collections.ObjectModel.ObservableCollection<string>();
            _recurseOnDeleteGeneratedFiles = source.RecurseOnDeleteGeneratedFiles;
            _lastGeneratedFilesFilename = source.LastGeneratedFilesFilename;
            _encoding = source.Encoding;
            _path = source.Path;
            _cultureInfo = source.CultureInfo;
            _generateMultipleFiles = source.GenerateMultipleFiles;
            _skipWhenFileExists = source.SkipWhenFileExists;
            _createCodeGenerationHeader = source.CreateCodeGenerationHeader;
            _environmentVersion = source.EnvironmentVersion;
            _filenameSuffix = source.FilenameSuffix;
            _enableNullableContext = source.EnableNullableContext;
            _enableNullablePragmas = source.EnableNullablePragmas;
            _enableGlobalUsings = source.EnableGlobalUsings;
            if (source.CustomUsings is not null) foreach (var item in source.CustomUsings) _customUsings.Add(item);
            if (source.NamespacesToAbbreviate is not null) foreach (var item in source.NamespacesToAbbreviate) _namespacesToAbbreviate.Add(item);
        }

        public CsharpClassGeneratorSettingsBuilder()
        {
            _customUsings = new System.Collections.ObjectModel.ObservableCollection<string>();
            _namespacesToAbbreviate = new System.Collections.ObjectModel.ObservableCollection<string>();
            _lastGeneratedFilesFilename = string.Empty;
            _encoding = default(System.Text.Encoding)!;
            _path = string.Empty;
            _cultureInfo = default(System.Globalization.CultureInfo)!;
            _environmentVersion = string.Empty;
            _filenameSuffix = string.Empty;
            _enableNullablePragmas = true;
            SetDefaultValues();
        }

        public ClassFramework.TemplateFramework.CsharpClassGeneratorSettings Build()
        {
            return new ClassFramework.TemplateFramework.CsharpClassGeneratorSettings(RecurseOnDeleteGeneratedFiles, LastGeneratedFilesFilename, Encoding, Path, CultureInfo, GenerateMultipleFiles, SkipWhenFileExists, CreateCodeGenerationHeader, EnvironmentVersion, FilenameSuffix, EnableNullableContext, EnableNullablePragmas, EnableGlobalUsings, CustomUsings, NamespacesToAbbreviate);
        }

        partial void SetDefaultValues();

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder AddCustomUsings(System.Collections.Generic.IEnumerable<string> customUsings)
        {
            if (customUsings is null) throw new System.ArgumentNullException(nameof(customUsings));
            return AddCustomUsings(customUsings.ToArray());
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder AddCustomUsings(params string[] customUsings)
        {
            if (customUsings is null) throw new System.ArgumentNullException(nameof(customUsings));
            foreach (var item in customUsings) CustomUsings.Add(item);
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder AddNamespacesToAbbreviate(System.Collections.Generic.IEnumerable<string> namespacesToAbbreviate)
        {
            if (namespacesToAbbreviate is null) throw new System.ArgumentNullException(nameof(namespacesToAbbreviate));
            return AddNamespacesToAbbreviate(namespacesToAbbreviate.ToArray());
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder AddNamespacesToAbbreviate(params string[] namespacesToAbbreviate)
        {
            if (namespacesToAbbreviate is null) throw new System.ArgumentNullException(nameof(namespacesToAbbreviate));
            foreach (var item in namespacesToAbbreviate) NamespacesToAbbreviate.Add(item);
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithRecurseOnDeleteGeneratedFiles(bool recurseOnDeleteGeneratedFiles = true)
        {
            RecurseOnDeleteGeneratedFiles = recurseOnDeleteGeneratedFiles;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithLastGeneratedFilesFilename(string lastGeneratedFilesFilename)
        {
            if (lastGeneratedFilesFilename is null) throw new System.ArgumentNullException(nameof(lastGeneratedFilesFilename));
            LastGeneratedFilesFilename = lastGeneratedFilesFilename;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithEncoding(System.Text.Encoding encoding)
        {
            if (encoding is null) throw new System.ArgumentNullException(nameof(encoding));
            Encoding = encoding;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithPath(string path)
        {
            if (path is null) throw new System.ArgumentNullException(nameof(path));
            Path = path;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithCultureInfo(System.Globalization.CultureInfo cultureInfo)
        {
            if (cultureInfo is null) throw new System.ArgumentNullException(nameof(cultureInfo));
            CultureInfo = cultureInfo;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithGenerateMultipleFiles(bool generateMultipleFiles = true)
        {
            GenerateMultipleFiles = generateMultipleFiles;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithSkipWhenFileExists(bool skipWhenFileExists = true)
        {
            SkipWhenFileExists = skipWhenFileExists;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithCreateCodeGenerationHeader(bool createCodeGenerationHeader = true)
        {
            CreateCodeGenerationHeader = createCodeGenerationHeader;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithEnvironmentVersion(string environmentVersion)
        {
            if (environmentVersion is null) throw new System.ArgumentNullException(nameof(environmentVersion));
            EnvironmentVersion = environmentVersion;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithFilenameSuffix(string filenameSuffix)
        {
            if (filenameSuffix is null) throw new System.ArgumentNullException(nameof(filenameSuffix));
            FilenameSuffix = filenameSuffix;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithEnableNullableContext(bool enableNullableContext = true)
        {
            EnableNullableContext = enableNullableContext;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithEnableNullablePragmas(bool enableNullablePragmas = true)
        {
            EnableNullablePragmas = enableNullablePragmas;
            return this;
        }

        public ClassFramework.TemplateFramework.Builders.CsharpClassGeneratorSettingsBuilder WithEnableGlobalUsings(bool enableGlobalUsings = true)
        {
            EnableGlobalUsings = enableGlobalUsings;
            return this;
        }

        protected void HandlePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
#nullable disable
