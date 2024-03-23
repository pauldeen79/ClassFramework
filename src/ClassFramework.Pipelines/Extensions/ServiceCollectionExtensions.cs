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
            .AddScoped<IPipelineBuilder<IConcreteTypeBuilder, BuilderContext>, Builder.PipelineBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.ValidationFeatureBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.AbstractBuilderFeatureBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.AddAttributesFeatureBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.AddBuildMethodFeatureBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.AddCopyConstructorFeatureBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.AddDefaultConstructorFeatureBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.AddFluentMethodsForCollectionPropertiesFeatureBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.AddFluentMethodsForNonCollectionPropertiesFeatureBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.AddInterfacesFeatureBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.AddPropertiesFeatureBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.BaseClassFeatureBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.GenericsFeatureBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.ObservableFeatureBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.PartialFeatureBuilder>()
            .AddScoped<IBuilderFeatureBuilder, Builder.Features.SetNameFeatureBuilder>();

    private static IServiceCollection AddBuilderInterfacePipeline(this IServiceCollection services)
        => services
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<IConcreteTypeBuilder, BuilderExtensionContext>>().Build())
            .AddScoped<IPipelineBuilder<IConcreteTypeBuilder, BuilderExtensionContext>, BuilderExtension.PipelineBuilder>()
            .AddScoped<IBuilderExtensionFeatureBuilder, BuilderExtension.Features.ValidationFeatureBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IBuilderExtensionFeatureBuilder, BuilderExtension.Features.AddExtensionMethodsForCollectionPropertiesFeatureBuilder>()
            .AddScoped<IBuilderExtensionFeatureBuilder, BuilderExtension.Features.AddExtensionMethodsForNonCollectionPropertiesFeatureBuilder>()
            .AddScoped<IBuilderExtensionFeatureBuilder, BuilderExtension.Features.PartialFeatureBuilder>()
            .AddScoped<IBuilderExtensionFeatureBuilder, BuilderExtension.Features.SetNameFeatureBuilder>()
            .AddScoped<IBuilderExtensionFeatureBuilder, BuilderExtension.Features.SetStaticFeatureBuilder>();

    private static IServiceCollection AddEntityPipeline(this IServiceCollection services)
        => services
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<IConcreteTypeBuilder, EntityContext>>().Build())
            .AddScoped<IPipelineBuilder<IConcreteTypeBuilder, EntityContext>, Entity.PipelineBuilder>()
            .AddScoped<IEntityFeatureBuilder, Entity.Features.ValidationFeatureBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IEntityFeatureBuilder, Entity.Features.AbstractEntityFeatureBuilder>()
            .AddScoped<IEntityFeatureBuilder, Entity.Features.AddAttributesFeatureBuilder>()
            .AddScoped<IEntityFeatureBuilder, Entity.Features.AddFullConstructorFeatureBuilder>()
            .AddScoped<IEntityFeatureBuilder, Entity.Features.AddGenericsFeatureBuilder>()
            .AddScoped<IEntityFeatureBuilder, Entity.Features.AddInterfacesFeatureBuilder>()
            .AddScoped<IEntityFeatureBuilder, Entity.Features.AddPropertiesFeatureBuilder>()
            .AddScoped<IEntityFeatureBuilder, Entity.Features.AddPublicParameterlessConstructorFeatureBuilder>()
            .AddScoped<IEntityFeatureBuilder, Entity.Features.AddToBuilderMethodFeatureBuilder>()
            .AddScoped<IEntityFeatureBuilder, Entity.Features.ObservableFeatureBuilder>()
            .AddScoped<IEntityFeatureBuilder, Entity.Features.PartialFeatureBuilder>()
            .AddScoped<IEntityFeatureBuilder, Entity.Features.SetBaseClassFeatureBuilder>()
            .AddScoped<IEntityFeatureBuilder, Entity.Features.SetNameFeatureBuilder>()
            .AddScoped<IEntityFeatureBuilder, Entity.Features.SetRecordFeatureBuilder>();

    private static IServiceCollection AddReflectionPipeline(this IServiceCollection services)
        => services
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<TypeBaseBuilder, Reflection.ReflectionContext>>().Build())
            .AddScoped<IPipelineBuilder<TypeBaseBuilder, Reflection.ReflectionContext>, Reflection.PipelineBuilder>()
            .AddScoped<IReflectionFeatureBuilder, Reflection.Features.ValidationFeatureBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IReflectionFeatureBuilder, Reflection.Features.AddAttributesFeatureBuilder>()
            .AddScoped<IReflectionFeatureBuilder, Reflection.Features.AddConstructorsFeatureBuilder>()
            .AddScoped<IReflectionFeatureBuilder, Reflection.Features.AddFieldsFeatureBuilder>()
            .AddScoped<IReflectionFeatureBuilder, Reflection.Features.AddGenericTypeArgumentsFeatureBuilder>()
            .AddScoped<IReflectionFeatureBuilder, Reflection.Features.AddInterfacesFeatureBuilder>()
            .AddScoped<IReflectionFeatureBuilder, Reflection.Features.AddMethodsFeatureBuilder>()
            .AddScoped<IReflectionFeatureBuilder, Reflection.Features.AddPropertiesFeatureBuilder>()
            .AddScoped<IReflectionFeatureBuilder, Reflection.Features.SetBaseClassFeatureBuilder>()
            .AddScoped<IReflectionFeatureBuilder, Reflection.Features.SetModifiersFeatureBuilder>()
            .AddScoped<IReflectionFeatureBuilder, Reflection.Features.SetNameFeatureBuilder>()
            .AddScoped<IReflectionFeatureBuilder, Reflection.Features.SetVisibilityFeatureBuilder>();

    private static IServiceCollection AddInterfacePipeline(this IServiceCollection services)
        => services
            .AddScoped(services => services.GetRequiredService<IPipelineBuilder<InterfaceBuilder, Interface.InterfaceContext>>().Build())
            .AddScoped<IPipelineBuilder<InterfaceBuilder, Interface.InterfaceContext>, Interface.PipelineBuilder>()
            .AddScoped<IInterfaceFeatureBuilder, Interface.Features.ValidationFeatureBuilder>() // important to register this one first, because validation should be performed first
            .AddScoped<IInterfaceFeatureBuilder, Interface.Features.AddAttributesFeatureBuilder>()
            .AddScoped<IInterfaceFeatureBuilder, Interface.Features.AddInterfacesFeatureBuilder>()
            .AddScoped<IInterfaceFeatureBuilder, Interface.Features.AddMethodsFeatureBuilder>()
            .AddScoped<IInterfaceFeatureBuilder, Interface.Features.AddPropertiesFeatureBuilder>()
            .AddScoped<IInterfaceFeatureBuilder, Interface.Features.SetNameFeatureBuilder>()
        ;

    private static IServiceCollection AddParserComponents(this IServiceCollection services)
        => services
            .AddScoped<IPlaceholderProcessor, BuilderPipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, BuilderInterfacePipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, EntityPipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, InterfacePipelinePlaceholderProcessor>()
            .AddScoped<IPlaceholderProcessor, ReflectionPipelinePlaceholderProcessor>();
}
