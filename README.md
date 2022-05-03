# Setup instructions for developers

This repo assumes Windows x64.

## Libraries and SDKS

### .NET Core 3.1

* Desktop Runtime for the GUI App. Direct download link for Windows: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-3.1.24-windows-x64-installer
* Console Runtime for running the presubmit tests from Git Hooks. https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-3.1.24-windows-x64-installer

## Path

Both vstest.console.exe and msbuild.exe must be in system path for the included scripts to function.
Find those executables using the `where` command:

`where /R C:\ vstest.console` will yield a result like C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\Extensions\TestPlatform\vstest.console.exe

`where /R C:\ msbuild` will yield a result like C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe

## Git Hooks

To run automated tests before each push, follow the instructions in pre-push.template.

# Automated Testing

Successful tests are automatically required before every publish. The publish action will compile and run the tests. Assuming the pre-push Git Hook was setup (see above), the tests are also built and run in the same way.