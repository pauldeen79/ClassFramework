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
namespace CrossCutting.Utilities.Parsers.Builders
{
    public abstract partial class FunctionCallArgumentBuilder<TBuilder, TEntity> : FunctionCallArgumentBuilder
        where TEntity : CrossCutting.Utilities.Parsers.FunctionCallArgument
        where TBuilder : FunctionCallArgumentBuilder<TBuilder, TEntity>
    {
        protected FunctionCallArgumentBuilder(CrossCutting.Utilities.Parsers.FunctionCallArgument source) : base(source)
        {
        }

        protected FunctionCallArgumentBuilder() : base()
        {
        }

        public override CrossCutting.Utilities.Parsers.FunctionCallArgument Build()
        {
            return BuildTyped();
        }

        public abstract TEntity BuildTyped();
    }
}
#nullable disable
