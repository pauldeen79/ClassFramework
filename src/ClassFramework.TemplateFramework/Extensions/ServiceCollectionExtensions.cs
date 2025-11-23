namespace ClassFramework.TemplateFramework.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddClassFrameworkTemplates(this IServiceCollection services)
        => services
            .AddTransient<CsharpClassGenerator>()
            .AddViewModel<AttributeViewModel>()
            .AddViewModel<ConstructorViewModel>()
            .AddViewModel<FieldViewModel>()
            .AddViewModel<MethodViewModel>()
            .AddViewModel<PropertyViewModel>()
            .AddViewModel<CodeGenerationHeaderViewModel>()
            .AddViewModel<EnumerationMemberViewModel>()
            .AddViewModel<EnumerationViewModel>()
            .AddViewModel<NewLineViewModel>()
            .AddViewModel<ParameterViewModel>()
            .AddViewModel<PropertyCodeBodyViewModel>()
            .AddViewModel<SpaceAndCommaViewModel>()
            .AddViewModel<TypeViewModel>()
            .AddViewModel<UsingsViewModel>()
            .AddViewModel<StringCodeStatementViewModel>()
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
            .AddChildTemplate<StringCodeStatementTemplate>(typeof(StringCodeStatement));

    public static IServiceCollection AddClassFrameworkCodeGenerators(this IServiceCollection services, IEnumerable<Type> generators)
    {
        Guard.IsNotNull(generators);

        foreach (var type in generators)
        {
            services.AddScoped(type);
        }

        return services;
    }
}
