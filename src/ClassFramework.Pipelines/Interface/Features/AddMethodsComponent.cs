﻿namespace ClassFramework.Pipelines.Interface.Features;

public class AddMethodsComponentBuilder : IInterfaceComponentBuilder
{
    public IPipelineComponent<InterfaceContext, InterfaceBuilder> Build()
        => new AddMethodsComponent();
}

public class AddMethodsComponent : IPipelineComponent<InterfaceContext, InterfaceBuilder>
{
    public Task<Result> Process(PipelineContext<InterfaceContext, InterfaceBuilder> context, CancellationToken token)
    {
        context = context.IsNotNull(nameof(context));

        if (!context.Request.Settings.CopyMethods)
        {
            return Task.FromResult(Result.Continue());
        }

        context.Response.AddMethods(context.Request.SourceModel.Methods
            .Where(x => context.Request.Settings.CopyMethodPredicate is null || context.Request.Settings.CopyMethodPredicate(context.Request.SourceModel, x))
            .Select(x => x.ToBuilder()
                .WithReturnTypeName(context.Request.MapTypeName(x.ReturnTypeName.FixCollectionTypeName(context.Request.Settings.EntityNewCollectionTypeName).FixNullableTypeName(new TypeContainerWrapper(x)), MetadataNames.CustomEntityInterfaceTypeName))
                .With(y => y.Parameters.ToList().ForEach(z => z.TypeName = context.Request.MapTypeName(z.TypeName, MetadataNames.CustomEntityInterfaceTypeName)))
            ));

        return Task.FromResult(Result.Continue());
    }
}

[ExcludeFromCodeCoverage]
internal sealed class TypeContainerWrapper : ITypeContainer
{
    public TypeContainerWrapper(Method method)
    {
        method = method.IsNotNull(nameof(method));
        TypeName = method.ReturnTypeName;
        IsNullable = method.ReturnTypeIsNullable;
        IsValueType = method.ReturnTypeIsValueType;
        GenericTypeArguments = method.ReturnTypeGenericTypeArguments;
    }

    public string TypeName { get; }
    public bool IsNullable { get; }
    public bool IsValueType { get; }

    public IReadOnlyCollection<ITypeContainer> GenericTypeArguments { get; }
}
