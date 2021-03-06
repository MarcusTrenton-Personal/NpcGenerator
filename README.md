# Setup instructions for developers

This repo assumes Windows x64.

## Libraries and SDKS

### .NET Core 3.1

* Desktop Runtime for the GUI App. Direct download link for Windows: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-3.1.24-windows-x64-installer
* Console Runtime for running the presubmit tests from Git Hooks. https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-3.1.24-windows-x64-installer

## Path

All the following must be in system path for the included scripts to function:
* vstest.console.exe (For running tests)
* msbuild.exe	(For building the tests before pushing)
* git.exe (For a tagging script)
Find those executables using the `where` command:

`where /R C:\ vstest.console` will yield a result like C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe

`where /R C:\ msbuild` will yield a result like C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Current\Bin\MSBuild.exe

`where /R C:\ git` will yield a result like C:\Users\Marcus\AppData\Local\Atlassian\SourceTree\git_local\bin\git.exe, if you use SourceTree for source control.

### Release Managers Only

Release Managers will also need to use Sign Tool for security.

`where /R C:\ signtool` will yield a result like C:\Program Files (x86)\Microsoft SDKs\ClickOnce\SignTool\signtool.exe

## Environmental Variables

You must set environmental variables before starting Visual Studio or the command line prompt.

Add a variable "Dropbox" for Dropbox's folder. This variable is used in the Dropbox publishing profile. 

Add a variable "TextTemplating" for the file path of Microsoft.TextTemplating.targets, which is needed for auto-incrementing the AssemblyFileVersion.
`where /R C:\ Microsoft.TextTemplating.targets` will yield a result like C:\Program Files\Microsoft Visual Studio\2022\Community\Msbuild\Microsoft\VisualStudio\v17.0\TextTemplating\Microsoft.TextTemplating.targets

### Release Managers Only

Release Managers will need to setup environmental variables for "SigningKey" file path and "SigningKeyPassword" for the password.

How do you actually get the signing key and password? Email MarcusTrenton@gmail.com with a good reason why you want to sign software in his name.

Without the signing key and password, the publishing targets will not succeed.

The public publishing profile will create a zipped file. Junction that folder to wherever you want the output to go (for example `mklink /J "D:\BackThisUp\ProgrammingProjects\Workspace\NpcGenerator\Zipped"  "H:\My Drive\Freeware\NpcGenerator"`. For the original author, the uncompressed files went to beta users in Dropbox while the zipped file was uploaded to Google Drive for public use.

## Git Hooks

To run automated tests before each push, follow the instructions in pre-push.template.

# Automated Testing

Successful tests are automatically required before every publish. The publish action will compile and run the tests. Assuming the pre-push Git Hook was setup (see above), the tests are also built and run in the same way.

# Dev Procedure

## Command Line Parameters
`-analyticsDryRun` will send Google Analytics to the validation server instead, providing a list of any errors with the message's json.

## Versioning

Versioning has the format Major.Minor.Day.Minute. 
- Major: Increment when backwards compatiblity is broken.
- Minor: Increment for feature additions and bug fixes. Recent to 0 when the Major number increments
- Day: Part of the build timestamp. It measures days since 2020/01/01. Automatically incremented per build.
- Minute: Part of the build timestamp. It measures minutes since midnight UTC time. Automatically incremented per build.

All of these are set in NpcGenerator/AssemblyInfo.tt. Set the Major and Minor versions in that file, not in the .csproj.

To publish, run PublishAndTag.bat. That script will automatically publish to Dropbox and tag the last commit.

## IDE Settings

Build before Run should be turned off due to text templating used for AssemblyInfo.cs. Otherwise debugging will break because the AssemblyInfo changed just before running. You need to build explicitely and then run explicitely.
In Visual Studio 2022 go to Tools -> Options -> Projects and Solutions -> Build And Run -> On Run, when projects are out of date: -> Never Build.
