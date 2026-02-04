@echo off
REM ============================================
REM Full Publish Script - Backend + Frontend
REM سكربت نشر كامل المشروع
REM ============================================

echo.
echo ================================================
echo    Role-Based Routing Demo - Full Publish
echo ================================================
echo.

REM Get current directory
set SCRIPT_DIR=%~dp0

echo [Step 1/2] Publishing Backend...
echo --------------------------------
call "%SCRIPT_DIR%publish-backend.bat"
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Backend publish failed!
    exit /b 1
)

echo.
echo [Step 2/2] Building Frontend...
echo --------------------------------
call "%SCRIPT_DIR%publish-frontend.bat"
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Frontend build failed!
    exit /b 1
)

echo.
echo ================================================
echo    All Done! Ready for Deployment
echo ================================================
echo.
echo Published files:
echo   Backend:  publish\backend\app\
echo   Frontend: publish\frontend\app\
echo.
echo See DEPLOYMENT.md for detailed instructions.
echo.

pause
