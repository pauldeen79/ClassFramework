namespace ClassFramework.Pipelines.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClassFrameworkPipelines(this IServiceCollection services)
        => services
            .AddBuilderPipeline()
            .AddBuilderExtensionPipeline()
            .AddEntityPipeline()
            .AddReflectionPipeline()
            .AddInterfacePipeline()
            .AddSharedPipelineComponents()
            .AddProcessingPipeline();

    private static IServiceCollection AddSharedPipelineComponents(this IServiceCollection services)
        => services
            .AddSingleton<IMember, ClassNameProperty>()
            .AddSingleton<IMember, ClassNamespaceProperty>()
            .AddSingleton<IMember, PropertyBuilderFuncPrefixProperty>()
            .AddSingleton<IMember, PropertyBuilderFuncSuffixProperty>()
            .AddSingleton<IMember, PropertyBuilderMemberNameProperty>()
            .AddSingleton<IMember, PropertyDefaultValueProperty>()
            .AddSingleton<IMember, PropertyEntityMemberNameProperty>()
            .AddSingleton<IMember, PropertyInitializationExpressionProperty>()
            .AddSingleton<IMember, PropertyNameProperty>()
            .AddSingleton<IMember, PropertyNullableRequiredSuffixProperty>()
            .AddSingleton<IMember, PropertyParentTypeFullNameProperty>()
            .AddSingleton<IMember, PropertyTypeNameProperty>()
            .AddSingleton<IMember, ArgumentNullCheckFunction>()
            .AddSingleton<IMember, ClassNameFunction>()
            .AddSingleton<IMember, CollectionItemTypeFunction>()
            .AddSingleton<IMember, CsharpFriendlyNameFunction>()
            .AddSingleton<IMember, CsharpFriendlyTypeNameFunction>()
            .AddSingleton<IMember, GenericArgumentsFunction>()
            .AddSingleton<IMember, InstancePrefixFunction>()
            .AddSingleton<IMember, NamespaceFunction>()
            .AddSingleton<IMember, NoGenericsFunction>()
            .AddSingleton<IMember, NoInterfacePrefixFunction>()
            .AddSingleton<IMember, NullCheckFunction>()
            .AddSingleton<IMember, SourceArgumentNullCheckFunction>()
            .AddSingleton<IMember, SourceNullCheckFunction>();

    private static IServiceCollection AddBuilderPipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandHandler, PipelineHandler<GenerateBuilderCommand, ClassBuilder>>()
            .AddScoped<ICommandHandler, ClassFrameworkCommandHandler<GenerateBuilderCommand, TypeBase>>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.AbstractBuilderComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.AddAttributesComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.AddBuildMethodComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.AddCopyConstructorComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.AddDefaultConstructorComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.AddFluentMethodsForCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.AddFluentMethodsForNonCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.AddImplicitOperatorComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.AddInterfacesComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.AddPropertiesComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.BaseClassComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.GenericsComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.ObservableComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderCommand, ClassBuilder>, Builder.Components.SetNameComponent>();

    private static IServiceCollection AddBuilderExtensionPipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandHandler, PipelineHandler<GenerateBuilderExtensionCommand, ClassBuilder>>()
            .AddScoped<ICommandHandler, ClassFrameworkCommandHandler<GenerateBuilderExtensionCommand, TypeBase>>()
            .AddScoped<IPipelineComponent<GenerateBuilderExtensionCommand, ClassBuilder>, BuilderExtension.Components.AddExtensionMethodsForCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderExtensionCommand, ClassBuilder>, BuilderExtension.Components.AddExtensionMethodsForNonCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderExtensionCommand, ClassBuilder>, BuilderExtension.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderExtensionCommand, ClassBuilder>, BuilderExtension.Components.SetNameComponent>()
            .AddScoped<IPipelineComponent<GenerateBuilderExtensionCommand, ClassBuilder>, BuilderExtension.Components.SetStaticComponent>();

    private static IServiceCollection AddEntityPipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandHandler, PipelineHandler<GenerateEntityCommand, ClassBuilder>>()
            .AddScoped<ICommandHandler, ClassFrameworkCommandHandler<GenerateEntityCommand, TypeBase>>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.AbstractEntityComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.AddAttributesComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.AddEquatableMembersComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.AddFullConstructorComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.AddGenericsComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.AddImplicitOperatorComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.AddInterfacesComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.AddPropertiesComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.AddPublicParameterlessConstructorComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.AddToBuilderMethodComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.ObservableComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.SetBaseClassComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.SetNameComponent>()
            .AddScoped<IPipelineComponent<GenerateEntityCommand, ClassBuilder>, Entity.Components.SetRecordComponent>();

    private static IServiceCollection AddReflectionPipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandHandler, PipelineHandler<GenerateTypeFromReflectionCommand, TypeBaseBuilder>>()
            .AddScoped<ICommandHandler, ClassFrameworkCommandHandler<GenerateTypeFromReflectionCommand, TypeBase>>()
            .AddScoped<IPipelineResponseGeneratorComponent, GenerateTypeFromReflectionResponseGeneratorComponent>()
            .AddScoped<IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>, Reflection.Components.AddAttributesComponent>()
            .AddScoped<IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>, Reflection.Components.AddConstructorsComponent>()
            .AddScoped<IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>, Reflection.Components.AddFieldsComponent>()
            .AddScoped<IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>, Reflection.Components.AddGenericTypeArgumentsComponent>()
            .AddScoped<IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>, Reflection.Components.AddInterfacesComponent>()
            .AddScoped<IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>, Reflection.Components.AddMethodsComponent>()
            .AddScoped<IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>, Reflection.Components.AddPropertiesComponent>()
            .AddScoped<IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>, Reflection.Components.SetBaseClassComponent>()
            .AddScoped<IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>, Reflection.Components.SetModifiersComponent>()
            .AddScoped<IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>, Reflection.Components.SetNameComponent>()
            .AddScoped<IPipelineComponent<GenerateTypeFromReflectionCommand, TypeBaseBuilder>, Reflection.Components.SetVisibilityComponent>();

    private static IServiceCollection AddInterfacePipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandHandler, PipelineHandler<GenerateInterfaceCommand, InterfaceBuilder>>()
            .AddScoped<ICommandHandler, ClassFrameworkCommandHandler<GenerateInterfaceCommand, TypeBase>>()
            .AddScoped<IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>, Interface.Components.AddAttributesComponent>()
            .AddScoped<IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>, Interface.Components.AddInterfacesComponent>()
            .AddScoped<IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>, Interface.Components.AddMethodsComponent>()
            .AddScoped<IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>, Interface.Components.AddPropertiesComponent>()
            .AddScoped<IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>, Interface.Components.GenericsComponent>()
            .AddScoped<IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>, Interface.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<GenerateInterfaceCommand, InterfaceBuilder>, Interface.Components.SetNameComponent>();

    private static IServiceCollection AddProcessingPipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandService, CommandService>()
            .AddScoped<ICommandDecorator>(_ => new ClassFrameworkCommandDecorator(new CrossCutting.Commands.PassThroughDecorator()))
            .AddScoped<IPipelineComponentDecorator>(_ => new CrossCutting.ProcessingPipeline.PassThroughDecorator())
            .AddScoped<IPipelineResponseGenerator, PipelineResponseGenerator>();
}
