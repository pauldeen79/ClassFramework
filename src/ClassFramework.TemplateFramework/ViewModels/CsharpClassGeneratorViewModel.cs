﻿namespace ClassFramework.TemplateFramework.ViewModels;

public class CsharpClassGeneratorViewModel : CsharpClassGeneratorViewModelBase<IEnumerable<TypeBase>>
{
    public IOrderedEnumerable<IGrouping<string, TypeBase>> Namespaces
        => Model.GroupBy(x => x.Namespace).OrderBy(x => x.Key);

    public CodeGenerationHeaderModel CodeGenerationHeaderModel
        => new(Settings.CreateCodeGenerationHeader, Settings.EnvironmentVersion);

    public UsingsModel Usings
        => new(Model);

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
    public IEnumerable<TypeBase> GetTypes(IEnumerable<TypeBase> @namespace)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        => @namespace.OrderBy(typeBase => typeBase.Name);

    public bool ShouldRenderNullablePragmas
    {
        get
        {
            var settings = Settings;

            return settings.EnableNullableContext
                && settings.EnableNullablePragmas
                && !settings.GenerateMultipleFiles; // only needed when generating single file
        }
    }
}
