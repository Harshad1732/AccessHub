# Records AccessHub demo GIF — starts API + Web, runs Playwright recorder, stops servers.
$ErrorActionPreference = "Stop"
$Root = Split-Path $PSScriptRoot -Parent
Set-Location $Root

Write-Host "Installing demo recorder dependencies..." -ForegroundColor Cyan
Push-Location scripts
npm install --silent
Pop-Location

Write-Host "Starting API on http://localhost:5177 ..." -ForegroundColor Cyan
$api = Start-Process -FilePath "dotnet" -ArgumentList "run","--project","src/AccessHub.Api","--no-launch-profile","--urls","http://localhost:5177" -PassThru -WindowStyle Hidden

Write-Host "Starting Web on http://localhost:5173 ..." -ForegroundColor Cyan
$web = Start-Process -FilePath "cmd.exe" -ArgumentList "/c","npm run dev -- --host" -WorkingDirectory "$Root\web" -PassThru -WindowStyle Hidden

try {
    Start-Sleep -Seconds 15
    Push-Location scripts
    npm run record
    Pop-Location
    Write-Host "`nDone! Add to README: docs/demo/accesshub-demo.gif" -ForegroundColor Green
}
finally {
    Write-Host "Stopping servers..." -ForegroundColor Cyan
    Stop-Process -Id $api.Id -Force -ErrorAction SilentlyContinue
    Stop-Process -Id $web.Id -Force -ErrorAction SilentlyContinue
    Get-Process -Name "node","dotnet" -ErrorAction SilentlyContinue | Where-Object { $_.Path -like "*IAM*" } | Stop-Process -Force -ErrorAction SilentlyContinue
}
