@echo off

REM Check if Powershell 7 is installed using winget
winget show Microsoft.Powershell >nul 2>&1
if %errorlevel% equ 0 (
    echo Powershell 7 is already installed.
    exit /b 0
)

REM Install or update Powershell 7 using winget
winget install --id Microsoft.Powershell --source winget
exit /b 0
