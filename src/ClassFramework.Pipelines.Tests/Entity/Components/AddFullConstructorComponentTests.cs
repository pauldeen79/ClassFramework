﻿namespace ClassFramework.Pipelines.Tests.Entity.Components;

public class AddFullConstructorComponentTests : TestBase<Pipelines.Entity.Components.AddFullConstructorComponent>
{
    public class ProcessAsync : AddFullConstructorComponentTests
    {
        [Fact]
        public async Task Throws_On_Null_Context()
        {
            // Arrange
            var sut = CreateSut();

            // Act & Assert
            var t = sut.ProcessAsync(context: null!);
            (await Should.ThrowAsync<ArgumentNullException>(t))
             .ParamName.ShouldBe("context");
        }

        [Fact]
        public async Task Adds_Constructor_Without_NullChecks()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addNullChecks: false);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.ShouldBeFalse();
            ctor.ChainCall.ShouldBeEmpty();
            ctor.Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property1", "property2", "property3" });
            ctor.Parameters.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "System.String", "System.Collections.Generic.IEnumerable<System.Int32>" });
            ctor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "this.Property1 = property1;",
                    "this.Property2 = property2;",
                    "this.Property3 = new System.Collections.Generic.List<System.Int32>(property3);"
                }
            );
        }

        [Fact]
        public async Task Adds_Constructor_With_NullChecks()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addNullChecks: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.ShouldBeFalse();
            ctor.ChainCall.ShouldBeEmpty();
            ctor.Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property1", "property2", "property3" });
            ctor.Parameters.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "System.String", "System.Collections.Generic.IEnumerable<System.Int32>" });
            ctor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "if (property2 is null) throw new System.ArgumentNullException(nameof(property2));",
                    "if (property3 is null) throw new System.ArgumentNullException(nameof(property3));",
                    "this.Property1 = property1;",
                    "this.Property2 = property2;",
                    "this.Property3 = new System.Collections.Generic.List<System.Int32>(property3);"
                }
            );
        }

        [Fact]
        public async Task Adds_Constructor_With_NullChecks_And_ExceptionThrowIfNull()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addNullChecks: true, useExceptionThrowIfNull: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.ShouldBeFalse();
            ctor.ChainCall.ShouldBeEmpty();
            ctor.Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property1", "property2", "property3" });
            ctor.Parameters.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "System.String", "System.Collections.Generic.IEnumerable<System.Int32>" });
            ctor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "System.ArgumentNullException.ThrowIfNull(property2);",
                    "System.ArgumentNullException.ThrowIfNull(property3);",
                    "this.Property1 = property1;",
                    "this.Property2 = property2;",
                    "this.Property3 = new System.Collections.Generic.List<System.Int32>(property3);"
                }
            );
        }

        [Fact]
        public async Task Adds_Constructor_With_NullChecks_And_BackingFields()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(addNullChecks: true, addBackingFields: true);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.ShouldBeFalse();
            ctor.ChainCall.ShouldBeEmpty();
            ctor.Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property1", "property2", "property3" });
            ctor.Parameters.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "System.String", "System.Collections.Generic.IEnumerable<System.Int32>" });
            ctor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "if (property2 is null) throw new System.ArgumentNullException(nameof(property2));",
                    "if (property3 is null) throw new System.ArgumentNullException(nameof(property3));",
                    "this._property1 = property1;",
                    "this._property2 = property2;",
                    "this._property3 = new System.Collections.Generic.List<System.Int32>(property3);"
                }
            );
        }

        [Fact]
        public async Task Adds_Constructor_With_DomainValidation()
        {
            // Arrange
            var sourceModel = CreateClass();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(validateArguments: ArgumentValidationType.IValidatableObject);
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.ShouldBeFalse();
            ctor.ChainCall.ShouldBeEmpty();
            ctor.Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo(new[] { "property1", "property2", "property3" });
            ctor.Parameters.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo(new[] { "System.Int32", "System.String", "System.Collections.Generic.IEnumerable<System.Int32>" });
            ctor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "this.Property1 = property1;",
                    "this.Property2 = property2;",
                    "this.Property3 = new System.Collections.Generic.List<System.Int32>(property3);",
                    "System.ComponentModel.DataAnnotations.Validator.ValidateObject(this, new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null), true);"
                }
            );
        }

        [Fact]
        public async Task Adds_Constructor_With_TypeMapping()
        {
            // Arrange
            var sourceModel = CreateClassWithCustomTypeProperties();
            await InitializeExpressionEvaluatorAsync();
            var sut = CreateSut();
            var settings = CreateSettingsForEntity(namespaceMappings: CreateNamespaceMappings());
            var context = new PipelineContext<EntityContext>(new EntityContext(sourceModel, settings, CultureInfo.InvariantCulture, CancellationToken.None));

            // Act
            var result = await sut.ProcessAsync(context);

            // Assert
            result.IsSuccessful().ShouldBeTrue();
            context.Request.Builder.Constructors.Count.ShouldBe(1);
            var ctor = context.Request.Builder.Constructors.Single();
            ctor.Protected.ShouldBeFalse();
            ctor.ChainCall.ShouldBeEmpty();
            ctor.Parameters.Select(x => x.Name).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "property1",
                    "property2",
                    "property3",
                    "property4",
                    "property5",
                    "property6",
                    "property7",
                    "property8"
                }
            );
            ctor.Parameters.Select(x => x.TypeName).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "System.Int32",
                    "System.Int32",
                    "System.String",
                    "System.String",
                    "MyNamespace.MyClass",
                    "MyNamespace.MyClass",
                    "System.Collections.Generic.IEnumerable<MyNamespace.MyClass>",
                    "System.Collections.Generic.IEnumerable<MyNamespace.MyClass>"
                }
            );
            ctor.CodeStatements.ShouldAllBe(x => x is StringCodeStatementBuilder);
            ctor.CodeStatements.OfType<StringCodeStatementBuilder>().Select(x => x.Statement).ToArray().ShouldBeEquivalentTo
            (
                new[]
                {
                    "this.Property1 = property1;",
                    "this.Property2 = property2;",
                    "this.Property3 = property3;",
                    "this.Property4 = property4;",
                    "this.Property5 = property5;",
                    "this.Property6 = property6;",
                    "this.Property7 = new System.Collections.Generic.List<MyNamespace.MyClass>(property7);",
                    "this.Property8 = property8 is null ? null : new System.Collections.Generic.List<MyNamespace.MyClass>(property8);"
                }
            );
        }
    }
}
