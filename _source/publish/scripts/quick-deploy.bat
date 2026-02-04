@echo off
REM ============================================
REM Quick Deploy Script
REM نشر سريع مع إنشاء ملفات ZIP
REM ============================================

echo.
echo ================================================
echo    Quick Deploy - Creating ZIP packages
echo ================================================
echo.

REM Run PowerShell script with CreateZip flag
powershell -ExecutionPolicy Bypass -File "%~dp0publish-all.ps1" -CreateZip

echo.
echo Done! Check the 'zip' folder for deployment packages.
echo.

pause
