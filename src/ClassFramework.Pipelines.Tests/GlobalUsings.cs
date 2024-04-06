﻿global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using AutoFixture;
global using AutoFixture.AutoNSubstitute;
global using AutoFixture.Kernel;
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
global using ClassFramework.Pipelines.Builder.Components.Abstractions;
global using ClassFramework.Pipelines.Builder.PlaceholderProcessors;
global using ClassFramework.Pipelines.BuilderExtension;
global using ClassFramework.Pipelines.BuilderExtension.Components.Abstractions;
global using ClassFramework.Pipelines.Builders;
global using ClassFramework.Pipelines.Domains;
global using ClassFramework.Pipelines.Entity;
global using ClassFramework.Pipelines.Entity.PlaceholderProcessors;
global using ClassFramework.Pipelines.Extensions;
global using ClassFramework.Pipelines.Interface;
global using ClassFramework.Pipelines.Reflection;
global using ClassFramework.Pipelines.Reflection.PlaceholderProcessors;
global using ClassFramework.Pipelines.Shared.PlaceholderProcessors;
global using ClassFramework.Pipelines.Tests.TestFixtures;
global using CrossCutting.Common;
global using CrossCutting.Common.DataAnnotations;
global using CrossCutting.Common.Extensions;
global using CrossCutting.Common.Results;
global using CrossCutting.ProcessingPipeline;
global using CrossCutting.Utilities.Parsers.Contracts;
global using CrossCutting.Utilities.Parsers.Extensions;
global using CsharpExpressionDumper.Abstractions;
global using CsharpExpressionDumper.Core.Extensions;
global using FluentAssertions;
global using Microsoft.Extensions.DependencyInjection;
global using NSubstitute;
global using Xunit;
