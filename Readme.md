# ClassFramework
Class modeling framework for C#

If you want to create a C# class structure based on a model, this framework is for you!

We are using the following dependencies:
- CsharpExpressionDumper, to generate c# code for values
- CrossCutting.ProcessingPipeline, for extendable pipelines for transformation of classes and interfaces
- CrossCutting.Utilities.Parsers, for parsing named format strings like ``{Name}Builder``
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

I have decided that the models for ClassFramework need to be generated using ClassFramework. To make this possible, I first generate everything with another framework (called ModelFramework) as a Bootstrap action. After this, I can re-generate everything using ClassFramework.

As with my other Github repositories, I have decided not to store generated code. So in the build pipeline, I need to run the code generation before I can build, test and pack the solution.
