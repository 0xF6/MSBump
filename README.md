
<p align="center">
  <a href="#">
    <img src="https://user-images.githubusercontent.com/13326808/121203343-b438fc00-c87e-11eb-8859-c9a9d5786e25.png">
  </a>
</p>
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

## Usage

1. Add the `Ivy.Versioning` NuGet package to your project.
2. Edit the project file. Make sure the file has a `<Version>` property.
3. Add the corresponding properties to the project (see next section).

## Settings
### `BumpMajor`, `BumpMinor`, `BumpPatch` and `BumpRevision`
These boolean properties control which part of the version is changed. 
To increment a specific part, add the corresponding property with true value.

Example - increment the revision number:
```xml
<PropertyGroup>
	<BumpRevision>true</BumpLabelDigits>
</PropertyGroup>
```
From an initial version of `1.0.0`, hitting build multiple times will change the version to `1.0.0.1`, `1.0.0.2`, etc.

### `BumpLabel` and `LabelDigits`
Using these properties, the task will add or increment a release label. Labels must be alphanumeric, and must not end in a digit. `LabelDigits` defaults to 6 if not specified.

Example - add a `dev` label with a 4-digit counter on every build:
```xml
<PropertyGroup>
	<BumpLabel>dev</BumpLabel>
	<BumpLabelDigits>4</BumpLabelDigits>
</PropertyGroup>
```

### `ResetMajor`, `ResetMinor`, `ResetPatch`, `ResetRevision` and `ResetLabel`

These properties will reset any part of the version. Major, Minor, Patch and Revision is reset to 0. When `ResetLabel` is used, the specified label is removed from the version.

Example - Increment the revision number on every Release build, add `dev` label with a 4-digit counter on Debug builds.
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

Reset properties are prioritized over Bump properties.

## Troubleshooting		

#### Problem	
```cmd
Ivy.Versioning.targets(6,5): error : Could not load file or assembly 'NuGet.Versioning, Version=5.5.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'.
```

#### Solution		
	
Replace		
```xml
<PackageReference Include="Ivy.Versioning" Version="3.1.0"> 
    <PrivateAssets>all</PrivateAssets> 
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets> 
</PackageReference> 
```		
to		
```xml
  <ItemGroup Condition="$(Configuration) == 'Debug'"> 
    <PackageReference Include="Ivy.Versioning" Version="3.1.0"> 
      <PrivateAssets>all</PrivateAssets> 
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets> 
    </PackageReference> 
  </ItemGroup> 
```


## Version history


### 3.1.0 (2020-04-04)

* Fix problem hen using .NET Core CLI 
* Fix writing XML version declaration into project file
* Remove support external config


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
