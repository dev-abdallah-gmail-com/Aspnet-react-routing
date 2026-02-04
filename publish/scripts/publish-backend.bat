@echo off
REM ============================================
REM Backend Publish Script for Windows
REM سكربت نشر الـ Backend على Windows
REM ============================================

echo.
echo ========================================
echo    Publishing ASP.NET Core Backend
echo ========================================
echo.

REM Set paths
set PROJECT_PATH=..\..\backend
set OUTPUT_PATH=..\backend\app
set CONFIG=Release

REM Check if dotnet is installed
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] .NET SDK is not installed!
    echo Please install from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [1/4] Cleaning previous build...
if exist "%OUTPUT_PATH%" rmdir /s /q "%OUTPUT_PATH%"

echo [2/4] Restoring packages...
dotnet restore "%PROJECT_PATH%\backend.csproj"
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Failed to restore packages!
    pause
    exit /b 1
)

echo [3/4] Publishing application...
dotnet publish "%PROJECT_PATH%\backend.csproj" -c %CONFIG% -o "%OUTPUT_PATH%" --self-contained false
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Failed to publish!
    pause
    exit /b 1
)

echo [4/4] Copying web.config...
copy /y "..\backend\web.config" "%OUTPUT_PATH%\web.config"

REM Create logs folder
if not exist "%OUTPUT_PATH%\logs" mkdir "%OUTPUT_PATH%\logs"

echo.
echo ========================================
echo    Backend Published Successfully!
echo ========================================
echo.
echo Output folder: %OUTPUT_PATH%
echo.
echo Next steps:
echo 1. Copy the 'app' folder to your IIS server
echo 2. Create a new IIS Application Pool (.NET CLR Version: No Managed Code)
echo 3. Create a new Website pointing to the 'app' folder
echo 4. Make sure ASP.NET Core Hosting Bundle is installed
echo.

pause
