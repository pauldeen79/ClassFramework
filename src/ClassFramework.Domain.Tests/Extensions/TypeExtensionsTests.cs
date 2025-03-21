namespace ClassFramework.Domain.Tests.Extensions;

public class TypeExtensionsTests
{
    public class WithoutGenerics
    {
        [Fact]
        public void Returns_GenericArgumentName_When_FullName_Is_Null()
        {
            // Arrange
            var sut = typeof(Nullable<>).GetGenericArguments()[0];

            // Act
            var actual = sut.WithoutGenerics();

            // Assert
            actual.ShouldBe("T");
        }

        [Fact]
        public void Returns_Same_TypeName_When_Type_Is_Not_GenericType()
        {
            // Arrange
            var sut = typeof(string);

            // Act
            var actual = sut.WithoutGenerics();

            // Assert
            actual.ShouldBe(typeof(string).FullName);
        }

        [Fact]
        public void Strips_Generics_When_Type_Is_GenericType()
        {
            // Arrange
            var sut = typeof(int?);

            // Act
            var actual = sut.WithoutGenerics();

            // Assert
            actual.ShouldBe("System.Nullable");
        }
    }

    public class GetTypeName
    {
        [Fact]
        public void Returns_Correct_Result_On_IReadOnlyCollection()
        {
            // Arrange
            var prop = GetType().GetProperty(nameof(MyProperty));
            var type = prop!.PropertyType;

            // Act
            var result = type.GetTypeName(prop);

            // Assert
            result.ShouldBe("System.Collections.Generic.IReadOnlyCollection<System.String>");
        }

        [Fact]
        public void Returns_Correct_Result_On_IReadOnlyCollection_With_Nullable_Argument()
        {
            // Arrange
            var prop = GetType().GetProperty(nameof(MyNullableProperty));
            var type = prop!.PropertyType;

            // Act
            var result = type.GetTypeName(prop);

            // Assert
            result.ShouldBe("System.Collections.Generic.IReadOnlyCollection<System.Object?>");
        }

        [Fact]
        public void Returns_Correct_Result_On_IReadOnlyCollection_With_Nullable_Type()
        {
            // Arrange
            var prop = GetType().GetProperty(nameof(MyNullableProperty2));
            var type = prop!.PropertyType;

            // Act
            var result = type.GetTypeName(prop);

            // Assert
            result.ShouldBe("System.Collections.Generic.IReadOnlyCollection<System.Object>?");
        }

        [Fact]
        public void Returns_Correct_Result_On_IReadOnlyCollection_With_Nested_Nullable_Argument()
        {
            // Arrange
            var prop = GetType().GetProperty(nameof(MyNullableProperty3));
            var type = prop!.PropertyType;

            // Act
            var result = type.GetTypeName(prop);

            // Assert
            result.ShouldBe("System.Collections.Generic.IReadOnlyCollection<System.Func<System.Object?>>");
        }

        [Fact]
        public void Returns_Correct_Result_On_Non_Collection_With_Nested_Nullable_Argument()
        {
            // Arrange
            var prop = GetType().GetProperty(nameof(MyNullableProperty4));
            var type = prop!.PropertyType;

            // Act
            var result = type.GetTypeName(prop);

            // Assert
            result.ShouldBe("System.Func<System.Collections.Generic.IEnumerable<System.Object?>>");
        }

        [Fact]
        public void Returns_Correct_Result_On_Enumerable_With_Non_Nullable_Argument()
        {
            // Arrange
            var prop = GetType().GetProperty(nameof(MyNullableProperty5));
            var type = prop!.PropertyType;

            // Act
            var result = type.GetTypeName(prop);

            // Assert
            result.ShouldBe("System.Collections.Generic.IEnumerable<System.String>");
        }

        [Fact]
        public void Returns_Correct_Result_On_Nullable_Int()
        {
            // Arrange
            var prop = GetType().GetProperty(nameof(MyProperty2));
            var type = prop!.PropertyType;

            // Act
            var result = type.GetTypeName(prop);

            // Assert
            result.ShouldBe("System.Nullable<System.Int32>");
        }

        [Fact]
        public void Returns_Correct_Result_On_Tuple_With_Nullable_And_NotNullable_ReferenceTypes()
        {
            // Arrange
            var prop = GetType().GetProperty(nameof(MyProperty3));
            var type = prop!.PropertyType;

            // Act
            var result = type.GetTypeName(prop);

            // Assert
            result.ShouldBe("System.Tuple<ClassFramework.Domain.Tests.Extensions.TypeExtensionsTests,System.Lazy<ClassFramework.Domain.Tests.Extensions.TypeExtensionsTests>>");
        }

        [Fact]
        public void Returns_Correct_Result_On_IReadOnlyCollection_With_Non_Nullable_Argument()
        {
            // Arrange
            var prop = GetType().GetProperty(nameof(MyProperty6));
            var type = prop!.PropertyType;

            // Act
            var result = type.GetTypeName(prop);

            // Assert
            result.ShouldBe("System.Collections.Generic.IReadOnlyCollection<ClassFramework.Domain.Tests.Extensions.TypeExtensionsTests.IMyObject>");
        }

        [Fact]
        public void Returns_Correct_Result_On_Func_With_Two_Nullable_Arguments()
        {
            // Arrange
            var prop = GetType().GetProperty(nameof(MyNullableProperty7));
            var type = prop!.PropertyType;

            // Act
            var result = type.GetTypeName(prop);

            // Assert
            result.ShouldBe("System.Func<System.Object?,System.String?>");
        }

        [Fact]
        public void Returns_Correct_Result_On_Func_With_Two_Nullable_Arguments_Nested()
        {
            // Arrange
            var prop = GetType().GetProperty(nameof(MyNullableProperty8));
            var type = prop!.PropertyType;

            // Act
            var result = type.GetTypeName(prop);

            // Assert
            result.ShouldBe("System.Func<System.Object?,System.Func<System.String?>>");
        }

        public IReadOnlyCollection<string> MyProperty { get; } = new ReadOnlyCollection<string>(Array.Empty<string>());
        [CsharpTypeName("System.Collections.Generic.IReadOnlyCollection<System.Object?>")]
        public IReadOnlyCollection<object?> MyNullableProperty { get; } = new ReadOnlyCollection<object?>(Array.Empty<object?>());
        [CsharpTypeName("System.Collections.Generic.IReadOnlyCollection<System.Object>?")]
        public IReadOnlyCollection<object>? MyNullableProperty2 { get; } = new ReadOnlyCollection<object>(Array.Empty<object>());
        [CsharpTypeName("System.Collections.Generic.IReadOnlyCollection<System.Func<System.Object?>>")]
        public IReadOnlyCollection<Func<object?>> MyNullableProperty3 { get; } = new ReadOnlyCollection<Func<object?>>(Array.Empty<Func<object?>>());
        [CsharpTypeName("System.Func<System.Collections.Generic.IEnumerable<System.Object?>>")]
        public Func<IEnumerable<object?>> MyNullableProperty4 { get; } = new Func<IEnumerable<object?>>(() => []);
        public IEnumerable<string> MyNullableProperty5 { get; } = default!;
        public IReadOnlyCollection<IMyObject> MyProperty6 { get; } = default!;
        public Func<object?, string?> MyNullableProperty7 { get; } = default!;
        [CsharpTypeName("System.Func<System.Object?,System.Func<System.String?>>")]
        public Func<object?, Func<string?>> MyNullableProperty8 { get; } = default!;
        public int? MyProperty2 { get; }
        public Tuple<TypeExtensionsTests, Lazy<TypeExtensionsTests>> MyProperty3 { get; } = null!;
    }

    public class GetGenericTypeArgumentsString
    {
        [Fact]
        public void Returns_Empty_String_When_Type_Is_Not_Generic()
        {
            // Arrange
            var type = typeof(string);

            // Act
            var result = type.GetGenericTypeArgumentsString();

            // Assert
            result.ShouldBeEmpty();
        }

        [Fact]
        public void Returns_Correct_Result_Without_Brackets()
        {
            // Arrange
            var type = typeof(Tuple<,>);

            // Act
            var result = type.GetGenericTypeArgumentsString(addBrackets: false);

            // Assert
            result.ShouldBe("T1,T2");
        }

        [Fact]
        public void Returns_Correct_Result_With_Brackets()
        {
            // Arrange
            var type = typeof(Tuple<,>);

            // Act
            var result = type.GetGenericTypeArgumentsString(addBrackets: true);

            // Assert
            result.ShouldBe("<T1,T2>");
        }
    }

    public interface IMyObject { object? Value { get; set; } }

    public class IsRecord
    {
        [Fact]
        public void Returns_False_On_Class()
        {
            // Arrange
            var sut = typeof(MyClass);

            // Act
            var result = sut.IsRecord();

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void Returns_False_On_Struct()
        {
            // Arrange
            var sut = typeof(MyStruct);

            // Act
            var result = sut.IsRecord();

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void Returns_False_On_Interface()
        {
            // Arrange
            var sut = typeof(IMyInterface);

            // Act
            var result = sut.IsRecord();

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public void Returns_True_On_Record()
        {
            // Arrange
            var sut = typeof(MyRecord);

            // Act
            var result = sut.IsRecord();

            // Assert
            result.ShouldBeTrue();
        }

#pragma warning disable S1144 // Unused private types or members should be removed
#pragma warning disable S2094 // Classes should not be empty
#pragma warning disable CA1812
        private sealed class MyClass { }
        private struct MyStruct { }
        private interface IMyInterface { }
        private sealed record MyRecord { }
#pragma warning restore CA1812
#pragma warning restore S2094 // Classes should not be empty
#pragma warning restore S1144 // Unused private types or members should be removed
    }

    public class GetFullName
    {
        [Fact]
        public void Returns_Full_Name_When_Namespace_Is_Present()
        {
            // Arrange
            var sut = new Class
            (
                "MyNamespace",
                default,
                [],
                [],
                [],
                [],
                default,
                "MyClass",
                [],
                [],
                [],
                [],
                default,
                default,
                default,
                [],
                default,
                string.Empty,
                [],
                []
            );

            // Act
            var result = sut.GetFullName();

            // Assert
            result.ShouldBe("MyNamespace.MyClass");
        }

        [Fact]
        public void Returns_Name_When_Namespace_Is_Not_Present()
        {
            // Arrange
            var sut = new Class
            (
                string.Empty,
                default,
                [],
                [],
                [],
                [],
                default,
                "MyClass",
                [],
                [],
                [],
                [],
                default,
                default,
                default,
                [],
                default,
                string.Empty,
                [],
                []
            );

            // Act
            var result = sut.GetFullName();

            // Assert
            result.ShouldBe("MyClass");
        }
    }

    public class ReplaceGenericTypeName
    {
        [Fact]
        public void Returns_Correct_Result_Using_Single_Type()
        {
            // Arrange
            var input = typeof(List<>);

            // Act
            var result = input.ReplaceGenericTypeName(typeof(string));

            // Assert
            result.ShouldBe("System.Collections.Generic.List<System.String>");
        }

        [Fact]
        public void Returns_Correct_Result_Using_Multiple_Types()
        {
            // Arrange
            var input = typeof(Dictionary<,>);

            // Act
            var result = input.ReplaceGenericTypeName(typeof(string), typeof(object));

            // Assert
            result.ShouldBe("System.Collections.Generic.Dictionary<System.String,System.Object>");
        }

        [Fact]
        public void Returns_Correct_Result_Using_Single_String()
        {
            // Arrange
            var input = typeof(List<>);

            // Act
            var result = input.ReplaceGenericTypeName(typeof(string).FullName!);

            // Assert
            result.ShouldBe("System.Collections.Generic.List<System.String>");
        }

        [Fact]
        public void Returns_Correct_Result_Using_Multiple_Strings()
        {
            // Arrange
            var input = typeof(Dictionary<,>);

            // Act
            var result = input.ReplaceGenericTypeName(typeof(string).FullName!, typeof(object).FullName!);

            // Assert
            result.ShouldBe("System.Collections.Generic.Dictionary<System.String,System.Object>");
        }
    }
}
