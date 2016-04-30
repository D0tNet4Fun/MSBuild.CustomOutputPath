# MSBuild.CustomOutputPath
This MSBuild extension allows customizing the output path when building a solution or project from inside Visual Studio or command line. It replaces the default relative directories Obj and Bin with a user-defined path, without changing the project files. This is designed to be used for speeding up the build process by using a different drive for MSbuild output, i.e. a RAM drive.

## Content
The extension consists of:
- a .props file which runs before Microsoft.Common.targets
- a .targets file which runs after Microsoft.Common.targets
- a .dll which contains the MSBuild tasks used by the targets

## How it works
The extension integrates with `Microsoft.Common.targets` and sets well-known properties such as `IntermediateOutputPath` and `OutDir` to be prefixed by the user-defined path also known as the **base output path**. The rest is handled by Microsoft.Common.targets.
The final paths may look like `$(BaseOutputPath)\MySolution\MyProject\obj\Debug`. Same for the `bin` directory.

## Features

#### Define the base output path
There are several ways to define the base output path. You can set the environment variable `MSBuildBaseOutputPath` or edit the .props file and change it there. The default value is _R:\Build_.

#### Link default target directory to the custom target directory [Visual Studio only]
When building from inside Visual Studio, the project's default target directory i.e. `bin\debug` will be converted to a symbolic link which points to the custom output directory. This ensures the IDE can run startup projects or unit test projects using the base output path.

Note: symlink creation requires _administrative privileges_ so this needs to run from an elevated process.

**How to undo this**: edit the .props file and make sure that property `BaseOutputPath` is not defined.

#### Extensibility
The extension allows local customization of the base output path. There are two targets file that can be defined in a solution's directory: `Before.CustomOutputPath.targets` and `After.CustomOutputPath.targets`. These files can contain solution-specific logic to further configure the output path.

Note that 'before' and 'after' are relative to the execution of `Microsoft.Common.targets` which means i.e. 'before' cannot use properties which are defined in `Microsoft.Common.targets` such as `$(TargetExt)`.
