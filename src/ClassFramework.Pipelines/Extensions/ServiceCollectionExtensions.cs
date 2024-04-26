namespace ClassFramework.Pipelines.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPipelines(this IServiceCollection services)
        => services
            .AddBuilderPipeline()
            .AddBuilderInterfacePipeline()
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
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<IConcreteTypeBuilder, BuilderContext>>().Build())
            .AddScoped<IPipelineBuilder<BuilderContext, IConcreteTypeBuilder>, Builder.PipelineBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.ValidationComponentBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IBuilderComponentBuilder, Builder.Features.AbstractBuilderComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.AddAttributesComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.AddBuildMethodComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.AddCopyConstructorComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.AddDefaultConstructorComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.AddFluentMethodsForCollectionPropertiesComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.AddFluentMethodsForNonCollectionPropertiesComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.AddInterfacesComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.AddPropertiesComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.BaseClassComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.GenericsComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.ObservableComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.PartialComponentBuilder>()
            .AddScoped<IBuilderComponentBuilder, Builder.Features.SetNameComponentBuilder>();

    private static IServiceCollection AddBuilderInterfacePipeline(this IServiceCollection services)
        => services
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<IConcreteTypeBuilder, BuilderExtensionContext>>().Build())
            .AddScoped<IPipelineBuilder<BuilderExtensionContext, IConcreteTypeBuilder>, BuilderExtension.PipelineBuilder>()
            .AddScoped<IBuilderExtensionComponentBuilder, BuilderExtension.Components.ValidationComponentBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IBuilderExtensionComponentBuilder, BuilderExtension.Components.AddExtensionMethodsForCollectionPropertiesComponentBuilder>()
            .AddScoped<IBuilderExtensionComponentBuilder, BuilderExtension.Components.AddExtensionMethodsForNonCollectionPropertiesComponentBuilder>()
            .AddScoped<IBuilderExtensionComponentBuilder, BuilderExtension.Components.PartialComponentBuilder>()
            .AddScoped<IBuilderExtensionComponentBuilder, BuilderExtension.Components.SetNameComponentBuilder>()
            .AddScoped<IBuilderExtensionComponentBuilder, BuilderExtension.Components.SetStaticComponentBuilder>();

    private static IServiceCollection AddEntityPipeline(this IServiceCollection services)
        => services
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<IConcreteTypeBuilder, EntityContext>>().Build())
            .AddScoped<IPipelineBuilder<EntityContext, IConcreteTypeBuilder>, Entity.PipelineBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Features.ValidationComponentBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IEntityComponentBuilder, Entity.Features.AbstractEntityComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Features.AddAttributesComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Features.AddFullConstructorComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Features.AddGenericsComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Features.AddInterfacesComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Features.AddPropertiesComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Features.AddPublicParameterlessConstructorComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Features.AddToBuilderMethodComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Features.ObservableComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Features.PartialComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Features.SetBaseClassComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Features.SetNameComponentBuilder>()
            .AddScoped<IEntityComponentBuilder, Entity.Features.SetRecordComponentBuilder>();

    private static IServiceCollection AddReflectionPipeline(this IServiceCollection services)
        => services
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<TypeBaseBuilder, Reflection.ReflectionContext>>().Build())
            .AddScoped<IPipelineBuilder<Reflection.ReflectionContext, TypeBaseBuilder>, Reflection.PipelineBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Features.ValidationComponentBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IReflectionComponentBuilder, Reflection.Features.AddAttributesComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Features.AddConstructorsComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Features.AddFieldsComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Features.AddGenericTypeArgumentsComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Features.AddInterfacesComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Features.AddMethodsComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Features.AddPropertiesComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Features.SetBaseClassComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Features.SetModifiersComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Features.SetNameComponentBuilder>()
            .AddScoped<IReflectionComponentBuilder, Reflection.Features.SetVisibilityComponentBuilder>();

    private static IServiceCollection AddInterfacePipeline(this IServiceCollection services)
        => services
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<InterfaceBuilder, Interface.InterfaceContext>>().Build())
            .AddScoped<IPipelineBuilder<Interface.InterfaceContext, InterfaceBuilder>, Interface.PipelineBuilder>()
            .AddScoped<IInterfaceComponentBuilder, Interface.Features.ValidationComponentBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IInterfaceComponentBuilder, Interface.Features.AddAttributesComponentBuilder>()
            .AddScoped<IInterfaceComponentBuilder, Interface.Features.AddInterfacesComponentBuilder>()
            .AddScoped<IInterfaceComponentBuilder, Interface.Features.AddMethodsComponentBuilder>()
            .AddScoped<IInterfaceComponentBuilder, Interface.Features.AddPropertiesComponentBuilder>()
            .AddScoped<IInterfaceComponentBuilder, Interface.Features.SetNameComponentBuilder>()
        ;

    private static IServiceCollection AddParserComponents(this IServiceCollection services)
        => services
            .AddScoped<IPlaceholderProcessor, BuilderPipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, BuilderInterfacePipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, EntityPipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, InterfacePipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, ReflectionPipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, PropertyProcessor>(); // needed for recursive calls to FormattableStringParser from PropertyProcessor...
}
