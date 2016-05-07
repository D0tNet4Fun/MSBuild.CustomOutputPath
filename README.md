# MSBuild.CustomOutputPath
This MSBuild extension allows changing the output paths when building a solution or project using Visual Studio or from the command line. It replaces the default relative directories Obj and Bin with user-defined paths without changing the project files. This is designed to be used for speeding up the build process by using a different drive for MSbuild output, i.e. a RAM drive.

## Content
The extension consists of:
- a .props file which defines the properties used by the extension
- a before.targets file which runs before Microsoft.Common.targets
- a after.targets file which runs after Microsoft.Common.targets
- a .dll which contains the MSBuild tasks used by the targets

## How it works
The extension integrates with `Microsoft.Common.targets` and sets well-known properties such as `IntermediateOutputPath` and `OutDir`. The values of these properties are set to begin with a user-defined path known as the **base output path**, followed by the name of the solution and the name of the project, i.e. `$(BaseOutputPath)\MySolution\MyProject\bin\Debug`. 

The base output path can be configured by setting the environment variable `MSBuildBaseOutputPath`, i.e. 'R:\Build', or by setting property `BaseOutputPath` from the command line. 
If the base output path is not set at build time then a _warning_ is issued and the build process will use the default output paths. If the base output path is set but the drive is N/A at build time, then the build fails with **error**.

## Features

#### Use links in output directory
This feature ensures that most of the files copied in the output directory are either hard links or symbolic links, instead of being actual copies of the original files. This saves disk space and minimizes the number of write operations, thus speeding up the build. This is enabled by default and it can be disabled by setting property `UseLinksInOutputDirectory` to `false`.

**Warning**: manual changes of linked output files propagate to the original files!


#### Link default output directory to the custom output directory [Visual Studio only]
When building from Visual Studio, the project's default output directory i.e. `bin\debug` will be converted to a symbolic link which points to the custom output directory. This ensures the IDE can run startup projects or unit test projects using the base output path.

**Known issue**: the first build of a startup project which is configured to use the _VS hosting process_ on debug will fail because the default output directory cannot be deleted while the process is running. As a workaround, disable the VS hosting process manually before the build and re-enable it after the build.

**How to undo this**: make sure that property `BaseOutputPath` is not defined.


#### MSBuild integration
The extension can be automatically imported by MSBuild using `Microsoft.Common.targets`'s before and after imports. It can also be imported manally. I.e.:
```xml
<Import Project="$(MSBuildExtensionsPath)\CustomOutputPathExtension\Before.CustomOutputPath.targets" />
<Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
<Import Project="$(MSBuildExtensionsPath)\CustomOutputPathExtension\After.CustomOutputPath.targets" />
```

Projects which do not import the common targets, such as [WiX Toolset](http://wixtoolset.org), have to integrate the extension targets manually. I.e.:
```xml
<PropertyGroup>
  <CustomBeforeWixTargets>$(MSBuildExtensionsPath)\CustomOutputPathExtension\Before.CustomOutputPath.targets</CustomBeforeWixTargets>
  <CustomAfterWixTargets>$(MSBuildExtensionsPath)\CustomOutputPathExtension\After.CustomOutputPath.targets</CustomAfterWixTargets>
</PropertyGroup>
```

#### ReSharper build
All features enabled when using Visual Studio are also enabled when running the [ReSharper build](https://blog.jetbrains.com/dotnet/2015/10/15/introducing-resharper-build) from Visual Studio.

## Extensibility
The extension allows solution-level customization of the output paths. There are two targets file that can be defined in a solution's directory: `Before.CustomOutputPath.targets` and `After.CustomOutputPath.targets`. These files can contain solution-specific logic to further configure the output path.

Note that 'before' and 'after' are relative to the execution of `Microsoft.Common.targets` which means i.e. 'before' cannot use properties which are defined in `Microsoft.Common.targets`, i.e. `$(TargetExt)`.

## Notes
Symbolic link creation requires **administrative privileges**. Make sure to run the extension in an elevated process.