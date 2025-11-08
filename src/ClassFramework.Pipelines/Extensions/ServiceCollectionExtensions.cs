using ClassFramework.Pipelines.PipelineResponseGeneratorComponents;

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
            .AddScoped<ICommandHandler, PipelineHandler<BuilderContext, ClassBuilder>>()
            .AddScoped<ICommandHandler, ContextCommandHandler<BuilderContext, ClassBuilder, TypeBase>>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.AbstractBuilderComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.AddAttributesComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.AddBuildMethodComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.AddCopyConstructorComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.AddDefaultConstructorComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.AddFluentMethodsForCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.AddFluentMethodsForNonCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.AddImplicitOperatorComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.AddInterfacesComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.AddPropertiesComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.BaseClassComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.GenericsComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.ObservableComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<BuilderContext, ClassBuilder>, Builder.Components.SetNameComponent>();

    private static IServiceCollection AddBuilderExtensionPipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandHandler, PipelineHandler<BuilderExtensionContext, ClassBuilder>>()
            .AddScoped<ICommandHandler, ContextCommandHandler<BuilderExtensionContext, ClassBuilder, TypeBase>>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext, ClassBuilder>, BuilderExtension.Components.AddExtensionMethodsForCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext, ClassBuilder>, BuilderExtension.Components.AddExtensionMethodsForNonCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext, ClassBuilder>, BuilderExtension.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext, ClassBuilder>, BuilderExtension.Components.SetNameComponent>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext, ClassBuilder>, BuilderExtension.Components.SetStaticComponent>();

    private static IServiceCollection AddEntityPipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandHandler, PipelineHandler<EntityContext, ClassBuilder>>()
            .AddScoped<ICommandHandler, ContextCommandHandler<EntityContext, ClassBuilder, TypeBase>>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.AbstractEntityComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.AddAttributesComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.AddEquatableMembersComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.AddFullConstructorComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.AddGenericsComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.AddImplicitOperatorComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.AddInterfacesComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.AddPropertiesComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.AddPublicParameterlessConstructorComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.AddToBuilderMethodComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.ObservableComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.SetBaseClassComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.SetNameComponent>()
            .AddScoped<IPipelineComponent<EntityContext, ClassBuilder>, Entity.Components.SetRecordComponent>();

    private static IServiceCollection AddReflectionPipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandHandler, PipelineHandler<Reflection.ReflectionContext>>()
            .AddScoped<ICommandHandler, ContextCommandHandler<Reflection.ReflectionContext, TypeBase>>()
            .AddScoped<IPipelineResponseGeneratorComponent, ReflectionContextPipelineResponseGeneratorComponent>()
            .AddScoped<IPipelineComponent<Reflection.ReflectionContext>, Reflection.Components.AddAttributesComponent>()
            .AddScoped<IPipelineComponent<Reflection.ReflectionContext>, Reflection.Components.AddConstructorsComponent>()
            .AddScoped<IPipelineComponent<Reflection.ReflectionContext>, Reflection.Components.AddFieldsComponent>()
            .AddScoped<IPipelineComponent<Reflection.ReflectionContext>, Reflection.Components.AddGenericTypeArgumentsComponent>()
            .AddScoped<IPipelineComponent<Reflection.ReflectionContext>, Reflection.Components.AddInterfacesComponent>()
            .AddScoped<IPipelineComponent<Reflection.ReflectionContext>, Reflection.Components.AddMethodsComponent>()
            .AddScoped<IPipelineComponent<Reflection.ReflectionContext>, Reflection.Components.AddPropertiesComponent>()
            .AddScoped<IPipelineComponent<Reflection.ReflectionContext>, Reflection.Components.SetBaseClassComponent>()
            .AddScoped<IPipelineComponent<Reflection.ReflectionContext>, Reflection.Components.SetModifiersComponent>()
            .AddScoped<IPipelineComponent<Reflection.ReflectionContext>, Reflection.Components.SetNameComponent>()
            .AddScoped<IPipelineComponent<Reflection.ReflectionContext>, Reflection.Components.SetVisibilityComponent>();

    private static IServiceCollection AddInterfacePipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandHandler, PipelineHandler<InterfaceContext, InterfaceBuilder>>()
            .AddScoped<ICommandHandler, ContextCommandHandler<InterfaceContext, InterfaceBuilder, TypeBase>>()
            .AddScoped<IPipelineComponent<InterfaceContext, InterfaceBuilder>, Interface.Components.AddAttributesComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext, InterfaceBuilder>, Interface.Components.AddInterfacesComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext, InterfaceBuilder>, Interface.Components.AddMethodsComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext, InterfaceBuilder>, Interface.Components.AddPropertiesComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext, InterfaceBuilder>, Interface.Components.GenericsComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext, InterfaceBuilder>, Interface.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext, InterfaceBuilder>, Interface.Components.SetNameComponent>();

    private static IServiceCollection AddProcessingPipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandService, CommandService>()
            .AddScoped<ICommandDecorator>(_ => new ValidateCommandDecorator(new CrossCutting.Commands.PassThroughDecorator()))
            .AddScoped<IPipelineComponentDecorator>(_ => new CrossCutting.ProcessingPipeline.PassThroughDecorator())
            .AddScoped<IPipelineResponseGenerator, PipelineResponseGenerator>();
}
