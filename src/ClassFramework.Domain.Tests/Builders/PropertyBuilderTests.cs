﻿namespace ClassFramework.Domain.Tests.Builders;

public class PropertyBuilderTests : TestBase<PropertyBuilder>
{
    public class Constructor : PropertyBuilderTests
    {
        [Fact]
        public void Sets_HasGetter_To_True()
        {
            // Act
            var sut = CreateSut();

            // Assert
            sut.HasGetter.Should().BeTrue();
        }

        [Fact]
        public void Sets_HasSetter_To_True()
        {
            // Act
            var sut = CreateSut();

            // Assert
            sut.HasSetter.Should().BeTrue();
        }
    }

    public class Instance : PropertyBuilderTests
    {
        // added to prove code generation is configured correctly.
        // we want to be able to validate builders before building, using default System.ComponentModel.DataAnnotations.Validator functionality
        // this can either be done using shared validation (which allows custom code using the IValidatableObject interface), or domain only validation with copied (validation) attributes (which does not allow custom validation code)
        [Fact]
        public void Can_Be_Validated_Using_Standard_Dotnet_Validation()
        {
            // Arrange
            var sut = CreateSut().WithName(string.Empty);
            var validationResults = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(sut, new ValidationContext(sut), validationResults);

            // Assert
            result.Should().BeFalse();
            validationResults.Should().NotBeEmpty();
        }

        [Fact]
        public void Can_Use_Shared_Validation_With_DomainEntity()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(int)).WithHasSetter().WithHasInitializer();
            var validationResults = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(sut, new ValidationContext(sut), validationResults);

            // Assert
            result.Should().BeFalse();
            validationResults.Should().NotBeEmpty();
            validationResults.Select(x => x.ErrorMessage).Should().BeEquivalentTo("HasSetter and HasInitializer cannot both be true");
        }
    }

    public class GetterNotImplemented : PropertyBuilderTests
    {
        [Fact]
        public void Adds_Correct_Statement()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.GetterNotImplemented();

            // Assert
            result.GetterCodeStatements.Should().BeEquivalentTo([new StringCodeStatementBuilder().WithStatement("throw new System.NotImplementedException();")]);
        }
    }

    public class SetterNotImplemented : PropertyBuilderTests
    {
        [Fact]
        public void Adds_Correct_Statement()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.SetterNotImplemented();

            // Assert
            result.SetterCodeStatements.Should().BeEquivalentTo([new StringCodeStatementBuilder().WithStatement("throw new System.NotImplementedException();")]);
        }
    }

    public class InitializerNotImplemented : PropertyBuilderTests
    {
        [Fact]
        public void Adds_Correct_Statement()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.InitializerNotImplemented();

            // Assert
            result.InitializerCodeStatements.Should().BeEquivalentTo([new StringCodeStatementBuilder().WithStatement("throw new System.NotImplementedException();")]);
        }
    }

    public class AddGetterStringGeCodeStatements : PropertyBuilderTests
    {
        [Fact]
        public void Adds_Correct_Statement()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddGetterStringCodeStatements(new[] { "// code goes here" }.AsEnumerable());

            // Assert
            result.GetterCodeStatements.Should().BeEquivalentTo([new StringCodeStatementBuilder().WithStatement("// code goes here")]);
        }
    }

    public class AddSetterStringGeCodeStatements : PropertyBuilderTests
    {
        [Fact]
        public void Adds_Correct_Statement()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddSetterStringCodeStatements(new[] { "// code goes here" }.AsEnumerable());

            // Assert
            result.SetterCodeStatements.Should().BeEquivalentTo([new StringCodeStatementBuilder().WithStatement("// code goes here")]);
        }
    }

    public class AddInitializerStringGeCodeStatements : PropertyBuilderTests
    {
        [Fact]
        public void Adds_Correct_Statement()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.AddInitializerStringCodeStatements(new[] { "// code goes here" }.AsEnumerable());

            // Assert
            result.InitializerCodeStatements.Should().BeEquivalentTo([new StringCodeStatementBuilder().WithStatement("// code goes here")]);
        }
    }
}
