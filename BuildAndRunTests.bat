@echo off

msbuild NpcGeneratorTests\NpcGeneratorTests.csproj
if NOT %errorlevel% == 0 exit /B %errorlevel%
vstest.console.exe NpcGeneratorTests\bin\Debug\netcoreapp3.1\NpcGeneratorTests.dll /Settings:NpcGeneratorTests\FullTestCleanup.runsettings