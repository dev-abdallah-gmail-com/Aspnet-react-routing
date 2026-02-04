@echo off
REM ============================================
REM Frontend Publish Script for Windows
REM سكربت نشر الـ Frontend على Windows
REM ============================================

echo.
echo ========================================
echo    Building React Frontend
echo ========================================
echo.

REM Set paths
set PROJECT_PATH=..\..\frontend
set OUTPUT_PATH=..\frontend\app

REM Check if Node.js is installed
where node >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Node.js is not installed!
    echo Please install from: https://nodejs.org/
    pause
    exit /b 1
)

echo [1/4] Cleaning previous build...
if exist "%OUTPUT_PATH%" rmdir /s /q "%OUTPUT_PATH%"

echo [2/4] Installing dependencies...
cd "%PROJECT_PATH%"
call npm install
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Failed to install dependencies!
    pause
    exit /b 1
)

echo [3/4] Building for production...
call npm run build
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Failed to build!
    pause
    exit /b 1
)

echo [4/4] Copying build files...
cd %~dp0
xcopy /s /e /y "%PROJECT_PATH%\dist\*" "%OUTPUT_PATH%\"

REM Copy web.config
copy /y "..\frontend\web.config" "%OUTPUT_PATH%\web.config"

echo.
echo ========================================
echo    Frontend Built Successfully!
echo ========================================
echo.
echo Output folder: %OUTPUT_PATH%
echo.
echo Next steps:
echo 1. Copy the 'app' folder to your IIS server
echo 2. Create a new Website pointing to the 'app' folder
echo 3. Make sure URL Rewrite Module is installed on IIS
echo.

pause
