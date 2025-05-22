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
            //.AddParserComponents()
            .AddPipelineService();

    private static IServiceCollection AddSharedPipelineComponents(this IServiceCollection services)
        => services
            //.AddScoped<IVariable, AddMethodNameFormatStringVariable>()
            //.AddScoped<IVariable, ClassVariable>()
            //.AddScoped<IVariable, CollectionTypeNameVariable>()
            //.AddScoped<IVariable, PropertyVariable>()
            .AddScoped<IFunction, ClassNameFunction>()
            .AddScoped<IFunction, CollectionItemTypeFunction>()
            .AddScoped<IFunction, CsharpFriendlyNameFunction>()
            .AddScoped<IFunction, CsharpFriendlyTypeNameFunction>()
            .AddScoped<IFunction, GenericArgumentsFunction>()
            .AddScoped<IFunction, InstancePrefixFunction>()
            .AddScoped<IFunction, NamespaceFunction>()
            .AddScoped<IFunction, NoGenericsFunction>()
            .AddScoped<IFunction, NoInterfacePrefixFunction>()
            .AddScoped<IFunction, NullCheckFunction>()
            //.AddScoped<IObjectResolverProcessor, ClassModelResolver>()
            //.AddScoped<IObjectResolverProcessor, CultureInfoResolver>()
            //.AddScoped<IObjectResolverProcessor, MappedContextBaseResolver>()
            //.AddScoped<IObjectResolverProcessor, PipelineSettingsResolver>()
            //.AddScoped<IObjectResolverProcessor, PropertyResolver>()
            //.AddScoped<IObjectResolverProcessor, TypeNameMapperResolver>()
        ;

    private static IServiceCollection AddBuilderPipeline(this IServiceCollection services)
        => services
            .AddScoped<IPipeline<BuilderContext>, Pipeline<BuilderContext>>()
            .AddScoped<IPipelineComponent<BuilderContext>, Builder.Components.ValidationComponent>() // important to register this one first, because validation should be performed first
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
            .AddScoped<IPipeline<BuilderExtensionContext>, Pipeline<BuilderExtensionContext>>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext>, BuilderExtension.Components.ValidationComponent>() // important to register this one first, because validation should be performed first
            .AddScoped<IPipelineComponent<BuilderExtensionContext>, BuilderExtension.Components.AddExtensionMethodsForCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext>, BuilderExtension.Components.AddExtensionMethodsForNonCollectionPropertiesComponent>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext>, BuilderExtension.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext>, BuilderExtension.Components.SetNameComponent>()
            .AddScoped<IPipelineComponent<BuilderExtensionContext>, BuilderExtension.Components.SetStaticComponent>();

    private static IServiceCollection AddEntityPipeline(this IServiceCollection services)
        => services
            .AddScoped<IPipeline<EntityContext>, Pipeline<EntityContext>>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.ValidationComponent>() // important to register this one first, because validation should be performed first
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AbstractEntityComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AddAttributesComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AddEquatableMembersComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AddFullConstructorComponent>()
            .AddScoped<IPipelineComponent<EntityContext>, Entity.Components.AddGenericsComponent>()
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
            .AddScoped<IPipeline<Reflection.ReflectionContext>, Pipeline<Reflection.ReflectionContext>>()
            .AddScoped<IPipelineComponent<Reflection.ReflectionContext>, Reflection.Components.ValidationComponent>() // important to register this one first, because validation should be performed first
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
            .AddScoped<IPipeline<InterfaceContext>, Pipeline<InterfaceContext>>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.ValidationComponent>() // important to register this one first, because validation should be performed first
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.AddAttributesComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.AddInterfacesComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.AddMethodsComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.AddPropertiesComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.GenericsComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.PartialComponent>()
            .AddScoped<IPipelineComponent<InterfaceContext>, Interface.Components.SetNameComponent>();

    //private static IServiceCollection AddParserComponents(this IServiceCollection services)
    //    => services
    //        .AddScoped<IPlaceholder, BuilderPipelinePlaceholderProcessor>()
    //        .AddScoped<IPlaceholder, BuilderExtensionPipelinePlaceholderProcessor>();

    private static IServiceCollection AddPipelineService(this IServiceCollection services)
        => services
            .AddScoped<IPipelineService, PipelineService>();
}
