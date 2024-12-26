# ClassFramework
Class modeling framework for C#

If you want to create a C# class structure based on a model, this framework is for you!

We are using the following dependencies:
- CsharpExpressionDumper, to generate c# code for values
- CrossCutting.ProcessingPipeline, for extendable pipelines for transformation of classes and interfaces
- CrossCutting.Utilities.Parsers, for parsing named format strings like ``{$property.Name}Builder``
- TemplateFramework, to translate the domain model to actual c# code files

The Domain and Pipelines packages target .NET Standard 2.0, so you can use it anywhere. Because of the dependency to TemplateFramework, we target some projects to .NET 8.0.

# Background

Imagine you want to create some domain model for a new project.

First, you need to create a console project named ``[YourProjectName].CodeGeneration``.

In this project, there are a number of components:
- The Program.cs class, which runs the code generation
- The Models folder, which contains interfaces for all your domain entities
- The CodeGenerationProviders folder, which contains all types of code generators, like entities, builders and maybe abstractions

# Philosophy

The entity and builder generators use the following characteristics:
- The entity can be either a POCO or a DDD-style object, which takes all input in the constructor and then validates the state
- You can use either immutable or mutable entities, and you can choose to make the setters private or public
- You can use observable properties on entities, if you need to
- For immutable entities, you can also generate builders. These are mutable, and you can easily convert between the two types (unless you want the entities in a separate project, without a reference to the builders)
- Validation is supported using the IValidatableObject interface (standard .NET validation), and can be shared between the entities and the builders
- Default values on the model properties can automatically be set in the constructor of the builders, if you want to
- You can either create entities without interfaces, or interface-based entities (possibly in a different namespace, e.g. MyProject.Core and MyProject.Abstractions)

# Dog fooding

I have decided that the models for ClassFramework need to be generated using ClassFramework. To make this possible, I have first generated everything with another framework (called ModelFramework) as a Bootstrap action. After this, I re-generated everything using ClassFramework.

But since version 0.9.1, I stored the generated code, so I could add breaking changes. This means the code generation in the build pipelines has been removed.

# Known issues

If you have types with nested or multiple generic arguments, the nullability of those types will not get determined correctly... Try to fix this by creating types for this.
E.g. System.Func<System.Object, System.Int32, System.Object?>
Just create a delegate for this, and add this to the type mapping.

If you still get errors, you could use the CsharpTypeNameAttribute to decorate your property/field/parameter.

# Upgrading to version 0.19

Most methods in the CsharpClassGeneratorPipelineCodeGenerationProviderBase class have wrapped their result in a Result<T> type.
So, for example, IEnumerable<TypeBase> becomes Result<IEnumerable<TypeBase>>.

For example, this code:
```C#
Task<IEnumerable<TypeBase>> GetModel()
```

has changed to:
```C#
Task<Result<IEnumerable<TypeBase>>>
```

This way, you can return errors and halt program flow with Result.Error instead of throwing exceptions.
