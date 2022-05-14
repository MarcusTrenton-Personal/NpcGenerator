@echo off

dotnet publish "NpcGenerator\NpcGenerator.csproj" /p:"PublishProfile=\NpcGenerator\Properties\PublishProfiles\Public.pubxml"
if errorlevel 1 exit /B errorlevel

for /f %%i in ('ExeVersion\bin\Release\netcoreapp3.1\ExeVersion NpcGenerator\bin\Release\netcoreapp3.1\publish\NpcGenerator.exe') do set VERSION=%%i

git tag v%VERSION%
git push origin v%VERSION%