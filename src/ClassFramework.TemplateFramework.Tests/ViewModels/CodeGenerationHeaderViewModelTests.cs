﻿namespace ClassFramework.TemplateFramework.Tests.ViewModels;

public class CodeGenerationHeaderViewModelTests : TestBase<CodeGenerationHeaderViewModel>
{
    public class Version : CodeGenerationHeaderViewModelTests
    {
        [Fact]
        public void Returns_Model_EnvironmentVersion_When_Filled()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new CodeGenerationHeaderModel(default, environmentVersion: "1.2.3.4");

            // Act
            var result = sut.Version;

            // Assert
            result.ShouldBe("1.2.3.4");
        }

        [Fact]
        public void Returns_Environment_Version_When_Model_EnvironmentVersion_Is_Not_Filled()
        {
            // Arrange
            var sut = CreateSut();
            sut.Model = new CodeGenerationHeaderModel(default, environmentVersion: default);

            // Act
            var result = sut.Version;

            // Assert
            result.ShouldBe(Environment.Version.ToString());
        }
    }
}
