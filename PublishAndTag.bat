@echo off
REM As a SourceTree custom action, for greatest convenience, enable the "Show Full Output" and "Run command silently" options

echo Publishing a Local build for final smoke testing
msbuild "NpcGenerator\NpcGenerator.csproj" -target:publish -property:PublishProfile=\NpcGenerator\Properties\PublishProfiles\Local.pubxml
if NOT %errorlevel% == 0 exit /B %errorlevel%

echo Testing whether local build can launch successfully
call CanAppLaunch NpcGenerator\bin\Release\netcoreapp3.1\publish\NpcGenerator.exe
if NOT %errorlevel% == 0 exit /B %errorlevel%

echo Publishing a public build
msbuild "NpcGenerator\NpcGenerator.csproj" -target:publish -property:PublishProfile=\NpcGenerator\Properties\PublishProfiles\Public.pubxml
if NOT %errorlevel% == 0 exit /B %errorlevel%

echo Extracting version number from the public build
REM The two builds could have been built with different minute-granular timestamps, making their versions different.
REM The public Dropbox version is the later one desired for tagging. 
REM Rather than recreate its complex, env-variable-based path, use the cached exe, as it will always be the last built exe.
REM Sadly batch files cannot directly assign the output of a command to a variable. This is an industry-standard work-around hack.
cd ExeVersion
dotnet restore
dotnet msbuild -p:Configuration=Release
if NOT %errorlevel% == 0 exit /B %errorlevel%
cd ..
for /f %%i in ('ExeVersion\bin\Release\netcoreapp3.1\ExeVersion ExeCache\NpcGenerator.exe') do set VERSION=%%i

echo Pushing a Git tag with the version number v%VERSION%
git tag v%VERSION%
if NOT %errorlevel% == 0 exit /B %errorlevel%
git push origin v%VERSION%
if NOT %errorlevel% == 0 exit /B %errorlevel%

echo New version successfully published