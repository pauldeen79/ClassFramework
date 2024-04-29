namespace ClassFramework.TemplateFramework.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClassFrameworkTemplates(this IServiceCollection services)
        => services
            // Add support for viewmodels in TemplateFramework
            .AddSingleton<ITemplateParameterConverter>(x => new ViewModelTemplateParameterConverter(() => x.GetServices<IViewModel>()))

            .AddTransient<CsharpClassGenerator>()
            .AddTransient<IViewModel, AttributeViewModel>()
            .AddTransient<IViewModel, ConstructorViewModel>()
            .AddTransient<IViewModel, FieldViewModel>()
            .AddTransient<IViewModel, MethodViewModel>()
            .AddTransient<IViewModel, PropertyViewModel>()
            .AddTransient<IViewModel, CodeGenerationHeaderViewModel>()
            .AddTransient<IViewModel, EnumerationMemberViewModel>()
            .AddTransient<IViewModel, EnumerationViewModel>()
            .AddTransient<IViewModel, NewLineViewModel>()
            .AddTransient<IViewModel, ParameterViewModel>()
            .AddTransient<IViewModel, PropertyCodeBodyViewModel>()
            .AddTransient<IViewModel, SpaceAndCommaViewModel>()
            .AddTransient<IViewModel, TypeViewModel>()
            .AddTransient<IViewModel, UsingsViewModel>()
            .AddTransient<IViewModel, StringCodeStatementViewModel>()
            .AddChildTemplate<AttributeTemplate>(typeof(Domain.Attribute))
            .AddChildTemplate<ConstructorTemplate>(typeof(Constructor))
            .AddChildTemplate<FieldTemplate>(typeof(Field))
            .AddChildTemplate<MethodTemplate>(typeof(Method))
            .AddChildTemplate<PropertyTemplate>(typeof(Property))
            .AddChildTemplate<CodeGenerationHeaderTemplate>(typeof(CodeGenerationHeaderModel))
            .AddChildTemplate<EnumerationMemberTemplate>(typeof(EnumerationMember))
            .AddChildTemplate<EnumerationTemplate>(typeof(Enumeration))
            .AddChildTemplate<NewLineTemplate>(typeof(NewLineModel))
            .AddChildTemplate<ParameterTemplate>(typeof(Parameter))
            .AddChildTemplate<PropertyCodeBodyTemplate>(typeof(PropertyCodeBodyModel))
            .AddChildTemplate<SpaceAndCommaTemplate>(typeof(SpaceAndCommaModel))
            .AddChildTemplate<TypeTemplate>(typeof(IType))
            .AddChildTemplate<UsingsTemplate>(typeof(UsingsModel))
            .AddChildTemplate<StringCodeStatementTemplate>(typeof(StringCodeStatement))

            // Add request handlers for using pipelines from CsharpClassGeneratorPipelineCodeGenerationProviderBase
            .AddTransient<IRequestHandler<PipelineRequest<BuilderExtensionContext, IConcreteType>, Result<IConcreteType>>, ClassFrameworkPipelineRequestHandler<IConcreteType, BuilderExtensionContext>>()
            .AddTransient<IRequestHandler<PipelineRequest<BuilderContext, TypeBase>, Result<TypeBase>>, ClassFrameworkPipelineRequestHandler<TypeBase, BuilderContext>>()
            .AddTransient<IRequestHandler<PipelineRequest<EntityContext, IConcreteType>, Result<IConcreteType>>, ClassFrameworkPipelineRequestHandler<IConcreteType, EntityContext>>()
            .AddTransient<IRequestHandler<PipelineRequest<InterfaceContext, Interface>, Result<Interface>>, ClassFrameworkPipelineRequestHandler<Interface, InterfaceContext>>()
            .AddTransient<IRequestHandler<PipelineRequest<ReflectionContext, TypeBase>, Result<TypeBase>>, ReflectionPipelineRequestHandler<TypeBase, ReflectionContext>>()
        ;
}
