# Nuclei
Nuclei is a collection of libraries containing classes and functions for inter-application interaction via a command mechanism, diagnostics, configuration handling, exception handling and assembly location and loading.
The `Nuclei` assembly contains a set of classes and interfaces for:

* An ID instance that is comparable and equatable.
* Extracting text and streams from an embedded resource.
* Comparison of `Type` instances.
* Loading of `Type` instances with a partial assembly name.
* Extension methods for `Assembly` instances.


# Installation instructions
All libraries are available on [NuGet.org](https://www.nuget.org/packages/Nuclei/).

# How to build
The solution files are created in Visual Studio 2013 (using .NET 4.0) and the assemblies can be build either from Visual Studio or through the build script.
To invoke the build script use the following command line from the workspace directory (assuming MsBuild is on the PATH)

    msbuild nuclei.msbuild /t:build

This will build the binaries and the NuGet package. The binaries will be placed in the `build\bin\AnyCpu\Release` directory and the NuGet package will be placed in the `build\deploy` directory.

Note that the build scripts assume that:

* The binaries should be signed, however the SNK key file is not included in the repository so a new key file has to be [created][snkfile_msdn]. The key file is referenced
  through an environment variable called `SOFTWARE_SIGNING_KEY_PATH` that has as value the directory path of the key file. The key file is expected to be called `nuclei.snk`
* GIT can be found on the PATH somewhere so that it can be called to get the hash of the last commit in the current repository. This hash is embedded in the assemblies together
  with information about the build configuration and build time and date.

# How to contribute
There are a few ways to contribute:

* By opening an issue on the project.
* By provide a pull-request for a new feature or a bug.

Any suggestions or improvements you may have are more than welcome.

[snkfile_msdn]: http://msdn.microsoft.com/en-us/library/6f05ezxy(v=vs.110).aspx