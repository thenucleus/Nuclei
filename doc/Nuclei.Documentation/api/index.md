
# Nuclei

DOCUMENTATION TO FOLLOW


## ID

The `ID<TId,TInternalValue>` type defines an immutable, serializable ID type that is comparable and has the equality methods and operators defined.

[!code-csharp[ExampleID](..\..\Nuclei.Samples\SampleId.cs?range=8-)]

The `ID<TId,TInternalValue>` type implements object equality through `IEquatable<T>` and the equality operators and object comparability through `IComparable` and `IComparable<T>`.


## Assembly extensions

The `AssemblyExtensions` class provides several extension methods for the `Assembly` type. Extension methods are provided to:

* Get the full file path of an assembly

[!code-csharp[Assembly.LocalFilePath](..\..\Nuclei.Samples\AssemblyExtensionsSample.cs?range=26)]

* Get the full directory path of an assembly

[!code-csharp[Assembly.LocalDirectoryPath](..\..\Nuclei.Samples\AssemblyExtensionsSample.cs?range=35)]

* Determine if the assembly is strong named

[!code-csharp[Assembly.IsStrongNamed](..\..\Nuclei.Samples\AssemblyExtensionsSample.cs?range=44-45)]

* Get the strong name of the assembly

[!code-csharp[Assembly.StrongName](..\..\Nuclei.Samples\AssemblyExtensionsSample.cs?range=44,49)]


## Embedded resources

The `EmbeddedResourceExtracter` class provides methods for extracting an embedded resource from a given assembly.

[!code-csharp[EmbeddedResourceExtracter.LoadEmbeddedStream](..\..\Nuclei.Samples\EmbeddedResourceExtracterSample.cs?range=30-35,38)]

[!code-csharp[EmbeddedResourceExtracter.LoadEmbeddedTextFile](..\..\Nuclei.Samples\EmbeddedResourceExtracterSample.cs?range=44-46)]


## Type

[!code-csharp[Type.IsAssignableToOpenGenericType](..\..\Nuclei.Samples\TypeSample.cs?range=26-27,30,32-33,36,38-39)]
