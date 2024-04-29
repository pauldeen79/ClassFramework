namespace ClassFramework.Pipelines.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPipelines(this IServiceCollection services)
        => services
            .AddBuilderPipeline()
            .AddBuilderExtensionPipeline()
            .AddEntityPipeline()
            .AddReflectionPipeline()
            .AddInterfacePipeline()
            .AddSharedPipelineComponents()
            .AddParserComponents();

    private static IServiceCollection AddSharedPipelineComponents(this IServiceCollection services)
        => services
            .AddScoped<IPipelinePlaceholderProcessor, PropertyProcessor>()
            .AddScoped<IPipelinePlaceholderProcessor, TypeBaseProcessor>()
            .AddScoped<IPipelinePlaceholderProcessor, TypeProcessor>();

    private static IServiceCollection AddBuilderPipeline(this IServiceCollection services)
        => services
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<BuilderContext>>().Build())
            .AddScoped<IPipelineBuilder<BuilderContext>, Builder.PipelineBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.ValidationComponentBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IBuilderComponentBuilder, Builder.Components.AbstractBuilderComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.AddAttributesComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.AddBuildMethodComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.AddCopyConstructorComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.AddDefaultConstructorComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.AddFluentMethodsForCollectionPropertiesComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.AddFluentMethodsForNonCollectionPropertiesComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.AddInterfacesComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.AddPropertiesComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.BaseClassComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.GenericsComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.ObservableComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.PartialComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Components.SetNameComponentBuilder>();

    private static IServiceCollection AddBuilderExtensionPipeline(this IServiceCollection services)
        => services
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<BuilderExtensionContext>>().Build())
            .AddScoped<IPipelineBuilder<BuilderExtensionContext>, BuilderExtension.PipelineBuilder>()
            .AddScoped<IBuilderExtensionComponentBuilder, BuilderExtension.Components.ValidationComponentBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IBuilderExtensionComponentBuilder, BuilderExtension.Components.AddExtensionMethodsForCollectionPropertiesComponentBuilder>()
            .AddScoped<IBuilderExtensionComponentBuilder, BuilderExtension.Components.AddExtensionMethodsForNonCollectionPropertiesComponentBuilder>()
            .AddScoped<IBuilderExtensionComponentBuilder, BuilderExtension.Components.PartialComponentBuilder>()
            .AddScoped<IBuilderExtensionComponentBuilder, BuilderExtension.Components.SetNameComponentBuilder>()
            .AddScoped<IBuilderExtensionComponentBuilder, BuilderExtension.Components.SetStaticComponentBuilder>();

    private static IServiceCollection AddEntityPipeline(this IServiceCollection services)
        => services
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<EntityContext>>().Build())
            .AddScoped<IPipelineBuilder<EntityContext>, Entity.PipelineBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Components.ValidationComponentBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IEntityComponentBuilder, Entity.Components.AbstractEntityComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Components.AddAttributesComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Components.AddFullConstructorComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Components.AddGenericsComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Components.AddInterfacesComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Components.AddPropertiesComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Components.AddPublicParameterlessConstructorComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Components.AddToBuilderMethodComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Components.ObservableComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Components.PartialComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Components.SetBaseClassComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Components.SetNameComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Components.SetRecordComponentBuilder>();

    private static IServiceCollection AddReflectionPipeline(this IServiceCollection services)
        => services
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<Reflection.ReflectionContext, TypeBaseBuilder>>().Build())
            .AddScoped<IPipelineBuilder<Reflection.ReflectionContext, TypeBaseBuilder>, Reflection.PipelineBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Components.ValidationComponentBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IReflectionComponentBuilder, Reflection.Components.AddAttributesComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Components.AddConstructorsComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Components.AddFieldsComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Components.AddGenericTypeArgumentsComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Components.AddInterfacesComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Components.AddMethodsComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Components.AddPropertiesComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Components.SetBaseClassComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Components.SetModifiersComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Components.SetNameComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Components.SetVisibilityComponentBuilder>();

    private static IServiceCollection AddInterfacePipeline(this IServiceCollection services)
        => services
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<Interface.InterfaceContext, InterfaceBuilder>>().Build())
            .AddScoped<IPipelineBuilder<Interface.InterfaceContext, InterfaceBuilder>, Interface.PipelineBuilder>()
            .AddScoped<IInterfaceComponentBuilder, Interface.Components.ValidationComponentBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IInterfaceComponentBuilder, Interface.Components.AddAttributesComponentBuilder>()
            .AddScoped<IInterfaceComponentBuilder, Interface.Components.AddInterfacesComponentBuilder>()
            .AddScoped<IInterfaceComponentBuilder, Interface.Components.AddMethodsComponentBuilder>()
            .AddScoped<IInterfaceComponentBuilder, Interface.Components.AddPropertiesComponentBuilder>()
            .AddScoped<IInterfaceComponentBuilder, Interface.Components.SetNameComponentBuilder>()
        ;

    private static IServiceCollection AddParserComponents(this IServiceCollection services)
        => services
            .AddScoped<IPlaceholderProcessor, BuilderPipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, BuilderExtensionPipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, EntityPipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, InterfacePipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, ReflectionPipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, PropertyProcessor>(); // needed for recursive calls to FormattableStringParser from PropertyProcessor...
}
