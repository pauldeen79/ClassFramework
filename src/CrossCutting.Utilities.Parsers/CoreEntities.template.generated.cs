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
namespace CrossCutting.Utilities.Parsers
{
    public partial record FormattableStringParserSettings
    {
        public System.IFormatProvider FormatProvider
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string PlaceholderStart
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string PlaceholderEnd
        {
            get;
        }

        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool EscapeBraces
        {
            get;
        }

        [System.ComponentModel.DefaultValueAttribute(10)]
        public int MaximumRecursion
        {
            get;
        }

        public FormattableStringParserSettings(System.IFormatProvider formatProvider, string placeholderStart, string placeholderEnd, bool escapeBraces, int maximumRecursion)
        {
            this.FormatProvider = formatProvider;
            this.PlaceholderStart = placeholderStart;
            this.PlaceholderEnd = placeholderEnd;
            this.EscapeBraces = escapeBraces;
            this.MaximumRecursion = maximumRecursion;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public CrossCutting.Utilities.Parsers.Builders.FormattableStringParserSettingsBuilder ToBuilder()
        {
            return new CrossCutting.Utilities.Parsers.Builders.FormattableStringParserSettingsBuilder(this);
        }
    }
    public partial record FunctionCall
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Name
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<CrossCutting.Utilities.Parsers.FunctionCallArgument> Arguments
        {
            get;
        }

        public FunctionCall(string name, System.Collections.Generic.IEnumerable<CrossCutting.Utilities.Parsers.FunctionCallArgument> arguments)
        {
            this.Name = name;
            this.Arguments = arguments is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<CrossCutting.Utilities.Parsers.FunctionCallArgument>(arguments);
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public CrossCutting.Utilities.Parsers.Builders.FunctionCallBuilder ToBuilder()
        {
            return new CrossCutting.Utilities.Parsers.Builders.FunctionCallBuilder(this);
        }
    }
    public partial record FunctionDescriptor
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Name
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Type FunctionType
        {
            get;
        }

        public System.Type? ReturnValueType
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Description
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<CrossCutting.Utilities.Parsers.FunctionDescriptorArgument> Arguments
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        [CrossCutting.Common.DataAnnotations.ValidateObjectAttribute]
        public System.Collections.Generic.IReadOnlyCollection<CrossCutting.Utilities.Parsers.FunctionDescriptorResult> Results
        {
            get;
        }

        public FunctionDescriptor(string name, System.Type functionType, System.Type? returnValueType, string description, System.Collections.Generic.IEnumerable<CrossCutting.Utilities.Parsers.FunctionDescriptorArgument> arguments, System.Collections.Generic.IEnumerable<CrossCutting.Utilities.Parsers.FunctionDescriptorResult> results)
        {
            this.Name = name;
            this.FunctionType = functionType;
            this.ReturnValueType = returnValueType;
            this.Description = description;
            this.Arguments = arguments is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<CrossCutting.Utilities.Parsers.FunctionDescriptorArgument>(arguments);
            this.Results = results is null ? null! : new CrossCutting.Common.ReadOnlyValueCollection<CrossCutting.Utilities.Parsers.FunctionDescriptorResult>(results);
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public CrossCutting.Utilities.Parsers.Builders.FunctionDescriptorBuilder ToBuilder()
        {
            return new CrossCutting.Utilities.Parsers.Builders.FunctionDescriptorBuilder(this);
        }
    }
    public partial record FunctionDescriptorArgument
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Name
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public System.Type Type
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Description
        {
            get;
        }

        public bool IsRequired
        {
            get;
        }

        public FunctionDescriptorArgument(string name, System.Type type, string description, bool isRequired)
        {
            this.Name = name;
            this.Type = type;
            this.Description = description;
            this.IsRequired = isRequired;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public CrossCutting.Utilities.Parsers.Builders.FunctionDescriptorArgumentBuilder ToBuilder()
        {
            return new CrossCutting.Utilities.Parsers.Builders.FunctionDescriptorArgumentBuilder(this);
        }
    }
    public partial record FunctionDescriptorResult
    {
        public CrossCutting.Common.Results.ResultStatus Status
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Value
        {
            get;
        }

        public System.Type? ValueType
        {
            get;
        }

        [System.ComponentModel.DataAnnotations.RequiredAttribute(AllowEmptyStrings = true)]
        public string Description
        {
            get;
        }

        public FunctionDescriptorResult(CrossCutting.Common.Results.ResultStatus status, string value, System.Type? valueType, string description)
        {
            this.Status = status;
            this.Value = value;
            this.ValueType = valueType;
            this.Description = description;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public CrossCutting.Utilities.Parsers.Builders.FunctionDescriptorResultBuilder ToBuilder()
        {
            return new CrossCutting.Utilities.Parsers.Builders.FunctionDescriptorResultBuilder(this);
        }
    }
}
#nullable disable
