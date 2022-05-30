@echo off
REM As a SourceTree custom action, for greatest convenience, enable the "Show Full Output" and "Run command silently" options

echo Publishing a Local build for final smoke testing
dotnet publish "NpcGenerator\NpcGenerator.csproj" /p:"PublishProfile=\NpcGenerator\Properties\PublishProfiles\Local.pubxml"
if errorlevel 1 exit /B errorlevel

echo Testing whether local build can launch successfully
call CanAppLaunch NpcGenerator\bin\Release\netcoreapp3.1\publish\NpcGenerator.exe
if errorlevel 1 exit /B errorlevel

echo Publishing a public build
dotnet publish "NpcGenerator\NpcGenerator.csproj" /p:"PublishProfile=\NpcGenerator\Properties\PublishProfiles\Public.pubxml"
if errorlevel 1 exit /B errorlevel

echo Extracting version number from the local build
REM Sadly batch files cannot directly assign the output of a command to a variable. This is an industry-standard work-around hack.
for /f %%i in ('ExeVersion\bin\Release\netcoreapp3.1\ExeVersion NpcGenerator\bin\Release\netcoreapp3.1\publish\NpcGenerator.exe') do set VERSION=%%i

echo Pushing a Git tag with the new verion number
git tag v%VERSION%
git push origin v%VERSION%

echo New version successfully published