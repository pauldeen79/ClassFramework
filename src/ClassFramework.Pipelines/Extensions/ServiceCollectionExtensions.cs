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
            .AddPipelineService();

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
            .AddScoped<ICommandHandler, Pipeline<BuilderContext>>()
            .AddScoped<ICommandHandler, ContextCommandHandler<BuilderContext, TypeBase>>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.AbstractBuilderComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.AddAttributesComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.AddBuildMethodComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.AddCopyConstructorComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.AddDefaultConstructorComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.AddFluentMethodsForCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.AddFluentMethodsForNonCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.AddImplicitOperatorComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.AddInterfacesComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.AddPropertiesComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.BaseClassComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.GenericsComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.ObservableComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.SetNameComponent>();

    private static IServiceCollection AddBuilderExtensionPipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandHandler, Pipeline<BuilderExtensionContext>>()
            .AddScoped<ICommandHandler, ContextCommandHandler<BuilderExtensionContext, TypeBase>>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext>, BuilderExtension.Components.AddExtensionMethodsForCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext>, BuilderExtension.Components.AddExtensionMethodsForNonCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext>, BuilderExtension.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext>, BuilderExtension.Components.SetNameComponent>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext>, BuilderExtension.Components.SetStaticComponent>();

    private static IServiceCollection AddEntityPipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandHandler, Pipeline<EntityContext>>()
            .AddScoped<ICommandHandler, ContextCommandHandler<EntityContext, TypeBase>>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AbstractEntityComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AddAttributesComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AddEquatableMembersComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AddFullConstructorComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AddGenericsComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AddImplicitOperatorComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AddInterfacesComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AddPropertiesComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AddPublicParameterlessConstructorComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AddToBuilderMethodComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.ObservableComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.SetBaseClassComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.SetNameComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.SetRecordComponent>();

    private static IServiceCollection AddReflectionPipeline(this IServiceCollection services)
        => services
            .AddScoped<ICommandHandler, Pipeline<Reflection.ReflectionContext>>()
            .AddScoped<ICommandHandler, ContextCommandHandler<Reflection.ReflectionContext, TypeBase>>()
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
            .AddScoped<ICommandHandler, Pipeline<InterfaceContext>>()
            .AddScoped<ICommandHandler, ContextCommandHandler<InterfaceContext, TypeBase>>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.AddAttributesComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.AddInterfacesComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.AddMethodsComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.AddPropertiesComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.GenericsComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.SetNameComponent>();

    private static IServiceCollection AddPipelineService(this IServiceCollection services)
        => services
            .AddScoped<ICommandService, CommandService>()
            .AddScoped<ICommandDecorator>(_ => new ValidateCommandDecorator(new CrossCutting.Commands.PassThroughDecorator()))
            .AddScoped<IPipelineComponentDecorator>(_ => new CrossCutting.ProcessingPipeline.PassThroughDecorator());
}
