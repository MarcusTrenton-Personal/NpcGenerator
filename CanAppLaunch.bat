@echo off
REM Parameter: Exe path

REM Adapted from https://stackoverflow.com/questions/9252980/how-to-split-the-filename-from-a-full-path-in-batch
For %%A in ("%1") do (
    Set Folder=%%~dpA
    Set Name=%%~nxA
)

start "Launch Test" %1

echo App should be launched. Checking tasklist.

REM Adapted from: https://stackoverflow.com/questions/162291/how-to-check-if-a-process-is-running-via-a-batch-script
tasklist /fi "ImageName eq %Name%" /fo csv 2>NUL | find /I "%Name%">NUL
if NOT "%ERRORLEVEL%"=="0" (exit /B 1)

echo App has launched. Killing app.

taskkill /IM "%Name%" /F

echo App test successful