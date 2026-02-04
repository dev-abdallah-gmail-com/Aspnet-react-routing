# ============================================
# Full Publish Script (PowerShell)
# سكربت نشر كامل المشروع
# ============================================

param(
    [string]$Configuration = "Release",
    [switch]$SkipBackend,
    [switch]$SkipFrontend,
    [switch]$CreateZip
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent (Split-Path -Parent $ScriptDir)
$PublishDir = Split-Path -Parent $ScriptDir

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "   Role-Based Routing Demo - Full Publish" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# ============================================
# Publish Backend
# ============================================
if (-not $SkipBackend) {
    Write-Host "[Backend] Starting publish..." -ForegroundColor Yellow

    $BackendProject = Join-Path $RootDir "backend\backend.csproj"
    $BackendOutput = Join-Path $PublishDir "backend\app"

    # Clean
    if (Test-Path $BackendOutput) {
        Remove-Item $BackendOutput -Recurse -Force
    }

    # Restore & Publish
    Write-Host "[Backend] Restoring packages..."
    dotnet restore $BackendProject

    Write-Host "[Backend] Publishing..."
    dotnet publish $BackendProject -c $Configuration -o $BackendOutput --self-contained false

    # Copy web.config
    Copy-Item (Join-Path $PublishDir "backend\web.config") $BackendOutput -Force

    # Create logs folder
    $LogsDir = Join-Path $BackendOutput "logs"
    if (-not (Test-Path $LogsDir)) {
        New-Item -ItemType Directory -Path $LogsDir | Out-Null
    }

    Write-Host "[Backend] Published successfully!" -ForegroundColor Green
}

# ============================================
# Build Frontend
# ============================================
if (-not $SkipFrontend) {
    Write-Host ""
    Write-Host "[Frontend] Starting build..." -ForegroundColor Yellow

    $FrontendDir = Join-Path $RootDir "frontend"
    $FrontendOutput = Join-Path $PublishDir "frontend\app"

    # Clean
    if (Test-Path $FrontendOutput) {
        Remove-Item $FrontendOutput -Recurse -Force
    }

    # Install & Build
    Push-Location $FrontendDir

    Write-Host "[Frontend] Installing dependencies..."
    npm install

    Write-Host "[Frontend] Building for production..."
    npm run build

    Pop-Location

    # Copy dist
    $DistDir = Join-Path $FrontendDir "dist"
    Copy-Item $DistDir $FrontendOutput -Recurse

    # Copy web.config
    Copy-Item (Join-Path $PublishDir "frontend\web.config") $FrontendOutput -Force

    Write-Host "[Frontend] Built successfully!" -ForegroundColor Green
}

# ============================================
# Create ZIP (Optional)
# ============================================
if ($CreateZip) {
    Write-Host ""
    Write-Host "[ZIP] Creating deployment packages..." -ForegroundColor Yellow

    $ZipDir = Join-Path $PublishDir "zip"
    if (-not (Test-Path $ZipDir)) {
        New-Item -ItemType Directory -Path $ZipDir | Out-Null
    }

    $Timestamp = Get-Date -Format "yyyyMMdd-HHmmss"

    # Backend ZIP
    $BackendZip = Join-Path $ZipDir "backend-$Timestamp.zip"
    Compress-Archive -Path (Join-Path $PublishDir "backend\app\*") -DestinationPath $BackendZip -Force
    Write-Host "[ZIP] Created: $BackendZip" -ForegroundColor Green

    # Frontend ZIP
    $FrontendZip = Join-Path $ZipDir "frontend-$Timestamp.zip"
    Compress-Archive -Path (Join-Path $PublishDir "frontend\app\*") -DestinationPath $FrontendZip -Force
    Write-Host "[ZIP] Created: $FrontendZip" -ForegroundColor Green
}

# ============================================
# Summary
# ============================================
Write-Host ""
Write-Host "================================================" -ForegroundColor Green
Write-Host "   Publish Completed Successfully!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Output folders:"
Write-Host "  Backend:  $PublishDir\backend\app\"
Write-Host "  Frontend: $PublishDir\frontend\app\"
Write-Host ""
Write-Host "Next steps:"
Write-Host "  1. Review DEPLOYMENT.md for instructions"
Write-Host "  2. Update configuration files for production"
Write-Host "  3. Deploy to your IIS server"
Write-Host ""
