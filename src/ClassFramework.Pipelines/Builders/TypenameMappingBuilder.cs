namespace ClassFramework.Pipelines.Builders;

public partial class TypenameMappingBuilder
{
    public TypenameMappingBuilder WithSourceType(Type sourceType) => WithSourceTypeName(sourceType.IsNotNull(nameof(sourceType)).FullName.FixTypeName());
    public TypenameMappingBuilder WithSourceType(IType sourceType) => WithSourceTypeName(sourceType.IsNotNull(nameof(sourceType)).GetFullName());
    public TypenameMappingBuilder WithSourceType(ITypeBuilder sourceType) => WithSourceTypeName(sourceType.IsNotNull(nameof(sourceType)).GetFullName());
    public TypenameMappingBuilder WithTargetType(Type targetType) => WithTargetTypeName(targetType.IsNotNull(nameof(targetType)).FullName.FixTypeName());
    public TypenameMappingBuilder WithTargetType(IType targetType) => WithTargetTypeName(targetType.IsNotNull(nameof(targetType)).GetFullName());
    public TypenameMappingBuilder WithTargetType(ITypeBuilder targetType) => WithTargetTypeName(targetType.IsNotNull(nameof(targetType)).GetFullName());
    public TypenameMappingBuilder AddMetadata(string name, object? value) => AddMetadata(new MetadataBuilder().WithName(name.IsNotNull(nameof(name))).WithValue(value));
}
