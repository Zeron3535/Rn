# Cleanup script for TinaKuafor project
# This script removes build artifacts and other unnecessary files before GitHub distribution

Write-Host "Starting cleanup process..." -ForegroundColor Green

# Remove bin and obj directories
Write-Host "Removing bin and obj directories..." -ForegroundColor Yellow
if (Test-Path bin) {
    Remove-Item -Path bin -Recurse -Force
    Write-Host "bin directory removed." -ForegroundColor Green
} else {
    Write-Host "bin directory not found." -ForegroundColor Yellow
}

if (Test-Path obj) {
    Remove-Item -Path obj -Recurse -Force
    Write-Host "obj directory removed." -ForegroundColor Green
} else {
    Write-Host "obj directory not found." -ForegroundColor Yellow
}

# Remove .vs directory if it exists
Write-Host "Removing .vs directory..." -ForegroundColor Yellow
if (Test-Path .vs) {
    Remove-Item -Path .vs -Recurse -Force
    Write-Host ".vs directory removed." -ForegroundColor Green
} else {
    Write-Host ".vs directory not found." -ForegroundColor Yellow
}

# Remove any SQLite temporary files
Write-Host "Removing SQLite temporary files..." -ForegroundColor Yellow
Remove-Item -Path "*.db-shm" -Force -ErrorAction SilentlyContinue
Remove-Item -Path "*.db-wal" -Force -ErrorAction SilentlyContinue
Write-Host "SQLite temporary files removed." -ForegroundColor Green

# Remove any user-specific files
Write-Host "Removing user-specific files..." -ForegroundColor Yellow
Remove-Item -Path "*.user" -Force -ErrorAction SilentlyContinue
Write-Host "User-specific files removed." -ForegroundColor Green

Write-Host "Cleanup completed successfully!" -ForegroundColor Green
Write-Host "The project is now ready for GitHub distribution." -ForegroundColor Green