﻿namespace ClassFramework.TemplateFramework.Tests.ViewModels;

public class EnumerationMemberViewModelTests : TestBase<EnumerationMemberViewModel>
{
    public class ValueExpression : EnumerationMemberViewModelTests
    {
        [Fact]
        public void Returns_Empty_String_When_Model_Value_Is_Null()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new EnumerationMemberBuilder().WithName("MyName").Build();

            // Act
            var result = sut.ValueExpression;

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
