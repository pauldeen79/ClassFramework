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
namespace ClassFramework.Domain.CodeStatements
{
    public partial record StringCodeStatement : ClassFramework.Domain.CodeStatementBase
    {
        [System.ComponentModel.DataAnnotations.RequiredAttribute]
        public string Statement
        {
            get;
        }

        public StringCodeStatement(string statement) : base()
        {
            this.Statement = statement;
            System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);
        }

        public override ClassFramework.Domain.Builders.CodeStatementBaseBuilder ToBuilder()
        {
            return ToTypedBuilder();
        }

        public ClassFramework.Domain.Builders.CodeStatements.StringCodeStatementBuilder ToTypedBuilder()
        {
            return new ClassFramework.Domain.Builders.CodeStatements.StringCodeStatementBuilder(this);
        }
    }
}
#nullable disable
