﻿namespace ClassFramework.Pipelines.Tests.Extensions;

public class ParentTypeContainerExtensionsTests : TestBase<PropertyBuilder>
{
    public class IsDefinedOn : ParentTypeContainerExtensionsTests
    {
        [Fact]
        public void Throws_On_Null_TypeBase_Argument()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(int)).Build();

            // Act & Assert
            Action a = () => sut.IsDefinedOn(typeBase: null!);
            a.ShouldThrow<ArgumentNullException>().ParamName.ShouldBe("typeBase");
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void Returns_Result_From_ComparisonFunction_When_Provided(bool comparisonFunctionResult, bool expectedResult)
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(int)).Build();
            var typeBase = new ClassBuilder().WithName("MyClass").Build();

            // Act
            var result = sut.IsDefinedOn(typeBase, comparisonDelegate: (_, _) => comparisonFunctionResult);

            // Assert
            result.ShouldBe(expectedResult);
        }

        [Fact]
        public void Returns_True_When_ParentTypeFullName_Is_Empty()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(int)).WithParentTypeFullName(string.Empty).Build();
            var typeBase = new ClassBuilder().WithName("MyClass").Build();

            // Act
            var result = sut.IsDefinedOn(typeBase);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Returns_True_When_ParentTypeFullName_Is_Equal_To_TypeBase_FullName()
        {
            // Arrange
            var typeBase = new ClassBuilder().WithName("MyClass").Build();
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(int)).WithParentType(typeBase).Build();

            // Act
            var result = sut.IsDefinedOn(typeBase);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Returns_True_When_ParentTypeFullName_Is_Equal_To_TypeBaseBuilder_FullName()
        {
            // Arrange
            var typeBase = new ClassBuilder().WithName("MyClass");
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(int)).WithParentType(typeBase).Build();

            // Act
            var result = sut.IsDefinedOn(typeBase.Build());

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public void Returns_False_When_ParentTypeFullName_Is_Filled_But_Not_Equal_To_TypeBase_FullName()
        {
            // Arrange
            var sut = CreateSut().WithName("MyProperty").WithType(typeof(int)).WithParentTypeFullName("SomeOtherType").Build();
            var typeBase = new ClassBuilder().WithName("MyClass").Build();

            // Act
            var result = sut.IsDefinedOn(typeBase);

            // Assert
            result.ShouldBeFalse();
        }
    }
}
