@echo off

dotnet restore
dotnet msbuild -p:Configuration=Debug
vstest.console.exe bin\Debug\netcoreapp3.1\Tests.dll /Settings:FullTestCleanup.runsettings