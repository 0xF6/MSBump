<!-- Logo -->
<p align="center">
  <a href="#">
    <img height="128" width="128" src="https://raw.githubusercontent.com/0xF6/Ivy.Versioning/master/images/icon.png">
  </a>
</p>

<!-- Name -->
<h1 align="center">
  ✨ Ivy.Versioning ✨
</h1>
<!-- desc -->
<h4 align="center">
  Ivy.Versioning is a MSBuild 16 task that bumps the version of a Visual Studio 2019 project.
</h4>
<p align="center">
  <a href="https://www.nuget.org/packages/Ivy.Versioning/">
    <img alt="Nuget" src="https://img.shields.io/nuget/v/Ivy.Versioning.svg?color=%23884499">
  </a>
  <a href="https://t.me/ivysola">
    <img alt="Telegram" src="https://img.shields.io/badge/Ask%20Me-Anything-1f425f.svg">
  </a>
</p>
<p align="center">
  <a href="#">
    <img src="https://forthebadge.com/images/badges/made-with-c-sharp.svg">
    <img src="https://forthebadge.com/images/badges/designed-in-ms-paint.svg">
    <img src="https://forthebadge.com/images/badges/ages-18.svg">
    <img src="https://ForTheBadge.com/images/badges/winter-is-coming.svg">
    <img src="https://forthebadge.com/images/badges/gluten-free.svg">
  </a>
</p>

Currently only tested on `.csproj` files.

## Usage

1. Add the `Ivy.Versioning` NuGet package to your project.
2. Edit the project file. Make sure the file has a `<Version>` property.
3. Create a `.ivy` settings file (see below) or add the corresponding properties to the project (see next section).

Warning: Until [this](https://github.com/NuGet/Home/issues/4125) NuGet issue is fixed, you should add `PrivateAssets="All"` to the `PackageReference` declaration,
otherwise your package will list `Ivy.Versioning` as a dependency.

Warning: When using .NET Core 3.1.**103** there may be problems when building and publishing via .NET CLI.

Warning: NuGet client before version 4.6 had an issue ([this](https://github.com/NuGet/Home/issues/4790)) that resulted in incorrect
dependency version numbers for P2P references in the generated NuGet packages. If you're experiencing this problem, try updating your NuGet client.
If updating the NuGet client is not an option, follow these steps:
1. Turn off "Generate NuGet package on build" in project properties.
2. Add `dotnet pack --no-build` as a post-build task.
    * For '.targets' files in multi-target solutions, set the 'AfterTargets' attribute to 'MSBumpAfterOuterBuild':
		```xml
		<Target Name="PackForBump" AfterTargets="MSBumpAfterOuterBuild">
		    <Exec Command="dotnet pack --no-build"/>
		</Target>
		```
After this, all your P2P references in the generated packages should have the correct (bumped) version after building the solution.

## Settings

MSBump settings can be declared in a separate `.ivy` file.
This file must be placed next to the project file, and must have the same name as the project file, but with the `.ivy` extension. Alternatively, it can be named simply as `.ivy`.
The file itself is a JSON file that contains the properties for the task object. 
When per-configuration settings are desireable, the settings file should be structured like this:
```js
{
  Configurations: {
    "Debug": {
      /* properties */
    },
    
    "Release": {
      /* properties */
    }
  }
}
```

Note that when a `.ivy` file is present, all other properties declared in `.targets` files are ignored for the current project. This is helpful when we use repository-wide MSBump configuration, but want to override this behavior for some projects. 

The settings file should contain any of the following properties:

### `BumpMajor`, `BumpMinor`, `BumpPatch` and `BumpRevision`
These boolean properties control which part of the version is changed. 
To increment a specific part, add the corresponding property with true value.

Example - increment the revision number:
```js
{
  BumpRevision: true
}    
```
From an initial version of `1.0.0`, hitting build multiple times will change the version to `1.0.0.1`, `1.0.0.2`, etc.

### `BumpLabel` and `LabelDigits`
Using these properties, the task will add or increment a release label. Labels must be alphanumeric, and must not end in a digit. `LabelDigits` defaults to 6 if not specified.

Example - add a `dev` label with a 4-digit counter on every build:
```js
{
  BumpLabel: "dev",
  LabelDigits: 4
}
```

### `ResetMajor`, `ResetMinor`, `ResetPatch`, `ResetRevision` and `ResetLabel`

These properties will reset any part of the version. Major, Minor, Patch and Revision is reset to 0. When `ResetLabel` is used, the specified label is removed from the version.

Example - Increment the revision number on every Release build, add `dev` label with a 4-digit counter on Debug builds.
```js
{
  Configurations: {
    "Debug": {
      BumpLabel: "dev",
      LabelDigits: 4
    },
    
    "Release": {
      BumpRevision: true,
      ResetLabel: "dev"
    }
  }
}
```

Reset properties are prioritized over Bump properties.

### Settings using project properties

When for some reason creating a `.msbump` file is not optimal - eg. when declaring these settings in `Directory.Build.targets` - the above properties can be placed inside the MSBuild project.
When doing so, use the below table to map the JSON properties to project properties:

|Property name|MSBuild project property|
|-------------|--------------------------------|
|BumpMajor|BumpMajor|
|BumpMinor|BumpMinor|
|BumpPatch|BumpPatch|
|BumpRevision|BumpRevision|
|BumpLabel|BumpLabel|
|LabelDigits|BumpLabelDigits|
|ResetMajor|BumpResetMajor|
|ResetMinor|BumpResetMinor|
|ResetPatch|BumpResetPatch|
|ResetRevision|BumpResetRevision|
|ResetLabel|BumpResetLabel|

Example - the previous example, using project properties

```xml
<PropertyGroup Condition="$(Configuration) == 'Debug'">
	<BumpLabel>dev</BumpLabel>
	<BumpLabelDigits>4</BumpLabelDigits>
</PropertyGroup>
<PropertyGroup Condition="$(Configuration) == 'Release'">
	<BumpRevision>True</BumpRevision>
	<BumpResetLabel>dev</BumpResetLabel>
</PropertyGroup>
```


## Version history


### 3.0.0 (2020-04-01)

* Upgrade all deps to lates
* Deploy new nuget package

### 2.3.2 (2017-12-13)

* Added support for multi-targeting projects
* Added support for projects with a default namespace (thanks to @jessyhoule)

### 2.3.0 (2017-08-19)

* .NET Standard support. MSBump now works with `dotnet build`.

### 2.2.0 (2017-08-15)

* Added support for settings file. No need to modify the project file at all when using the NuGet version.
* Cleaned up `.targets` files so that the standalone and NuGet version can work side by side.


### 2.1.0 (2017-08-12)

* MSBump now correctly bumps the version before build and pack, the built and packaged project always has the same version as the project file.

### 2.0.0 (2017-04-26)

* Added NuGet package
* `Major`, `Minor`, `Patch` and `Revision` are now simple boolean properties.

### 1.0.0
Initial standalone version
