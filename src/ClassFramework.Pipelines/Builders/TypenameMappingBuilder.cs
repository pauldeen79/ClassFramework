namespace ClassFramework.Pipelines.Builders;

public partial class TypenameMappingBuilder
{
    public TypenameMappingBuilder(Type type)
    {
        type = ArgumentGuard.IsNotNull(type, nameof(type));

        _sourceTypeName = type.FullName.FixTypeName();
        _targetTypeName = type.FullName.FixTypeName();
        _metadata = [];
    }

    public TypenameMappingBuilder(string typeName)
    {
        ArgumentGuard.IsNotNullOrEmpty(typeName, nameof(typeName));

        _sourceTypeName = typeName;
        _targetTypeName = typeName;
        _metadata = [];
    }

    public TypenameMappingBuilder(string sourceTypeName, string targetTypeName)
    {
        ArgumentGuard.IsNotNullOrEmpty(sourceTypeName, nameof(sourceTypeName));
        ArgumentGuard.IsNotNullOrEmpty(targetTypeName, nameof(targetTypeName));

        _sourceTypeName = sourceTypeName;
        _targetTypeName = targetTypeName;
        _metadata = [];
    }

    public TypenameMappingBuilder(Type sourceType, string targetTypeName)
    {
        sourceType = ArgumentGuard.IsNotNull(sourceType, nameof(sourceType));
        ArgumentGuard.IsNotNullOrEmpty(targetTypeName, nameof(targetTypeName));

        _sourceTypeName = sourceType.FullName.FixTypeName();
        _targetTypeName = targetTypeName;
        _metadata = [];
    }

    public TypenameMappingBuilder WithSourceType(Type sourceType) => WithSourceTypeName(sourceType.IsNotNull(nameof(sourceType)).FullName.FixTypeName());
    public TypenameMappingBuilder WithSourceType(IType sourceType) => WithSourceTypeName(sourceType.IsNotNull(nameof(sourceType)).GetFullName());
    public TypenameMappingBuilder WithSourceType(ITypeBuilder sourceType) => WithSourceTypeName(sourceType.IsNotNull(nameof(sourceType)).GetFullName());
    public TypenameMappingBuilder WithTargetType(Type targetType) => WithTargetTypeName(targetType.IsNotNull(nameof(targetType)).FullName.FixTypeName());
    public TypenameMappingBuilder WithTargetType(IType targetType) => WithTargetTypeName(targetType.IsNotNull(nameof(targetType)).GetFullName());
    public TypenameMappingBuilder WithTargetType(ITypeBuilder targetType) => WithTargetTypeName(targetType.IsNotNull(nameof(targetType)).GetFullName());
    public TypenameMappingBuilder AddMetadata(string name, object? value) => AddMetadata(new MetadataBuilder().WithName(name.IsNotNull(nameof(name))).WithValue(value));
}
