﻿global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Reflection;
global using System.Text;
global using ClassFramework.Domain;
global using ClassFramework.Domain.Abstractions;
global using ClassFramework.Domain.Builders;
global using ClassFramework.Domain.Builders.Abstractions;
global using ClassFramework.Domain.Builders.CodeStatements;
global using ClassFramework.Domain.Builders.Extensions;
global using ClassFramework.Domain.Builders.Types;
global using ClassFramework.Domain.Domains;
global using ClassFramework.Domain.Extensions;
global using ClassFramework.Domain.Types;
global using ClassFramework.Pipelines.Abstractions;
global using ClassFramework.Pipelines.Builder;
global using ClassFramework.Pipelines.Builder.Features.Abstractions;
global using ClassFramework.Pipelines.Builder.PlaceholderProcessors;
global using ClassFramework.Pipelines.BuilderExtension;
global using ClassFramework.Pipelines.BuilderExtension.Features.Abstractions;
global using ClassFramework.Pipelines.BuilderExtension.PlaceholderProcessors;
global using ClassFramework.Pipelines.Domains;
global using ClassFramework.Pipelines.Entity;
global using ClassFramework.Pipelines.Entity.Features.Abstractions;
global using ClassFramework.Pipelines.Entity.PlaceholderProcessors;
global using ClassFramework.Pipelines.Extensions;
global using ClassFramework.Pipelines.Interface.Features.Abstractions;
global using ClassFramework.Pipelines.Interface.PlaceholderProcessors;
global using ClassFramework.Pipelines.OverrideEntity;
global using ClassFramework.Pipelines.OverrideEntity.Features.Abstractions;
global using ClassFramework.Pipelines.OverrideEntity.PlaceholderProcessors;
global using ClassFramework.Pipelines.Reflection.Features.Abstractions;
global using ClassFramework.Pipelines.Reflection.PlaceholderProcessors;
global using ClassFramework.Pipelines.Shared.Features.Abstractions;
global using ClassFramework.Pipelines.Shared.PlaceholderProcessors;
global using CrossCutting.Common;
global using CrossCutting.Common.Extensions;
global using CrossCutting.Common.Results;
global using CrossCutting.ProcessingPipeline;
global using CrossCutting.Utilities.Parsers.Contracts;
global using CsharpExpressionDumper.Core;
global using Microsoft.Extensions.DependencyInjection;
