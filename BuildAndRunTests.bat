@echo off

msbuild ServicesTests\ServicesTests.csproj -verbosity:quiet
if NOT %errorlevel% == 0 exit /B %errorlevel%
vstest.console.exe ServicesTests\bin\Debug\netcoreapp3.1\ServicesTests.dll /Settings:TestUtils\FullTestCleanup.runsettings
if NOT %errorlevel% == 0 exit /B %errorlevel%

msbuild WpfServicesTests\WpfServicesTests.csproj -verbosity:quiet
if NOT %errorlevel% == 0 exit /B %errorlevel%
vstest.console.exe WpfServicesTests\bin\Debug\netcoreapp3.1\WpfServicesTests.dll /Settings:TestUtils\FullTestCleanup.runsettings
if NOT %errorlevel% == 0 exit /B %errorlevel%

msbuild NpcGeneratorTests\NpcGeneratorTests.csproj -verbosity:quiet
if NOT %errorlevel% == 0 exit /B %errorlevel%
vstest.console.exe NpcGeneratorTests\bin\Debug\netcoreapp3.1\NpcGeneratorTests.dll /Settings:TestUtils\FullTestCleanup.runsettings