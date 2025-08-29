#!/usr/bin/env pwsh

# Super quick benchmark runner script
# This runs minimal benchmarks for the fastest possible comparison

Write-Host "?? Super Quick Facet Benchmark Test" -ForegroundColor Green
Write-Host "=====================================" 
Write-Host "Minimal iterations for fastest possible comparison (< 1 minute)" -ForegroundColor Cyan
Write-Host ""

# Build in Release mode
Write-Host "Building in Release mode..." -ForegroundColor Yellow
dotnet build -c Release --verbosity quiet --nologo

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "? Build successful!" -ForegroundColor Green

# Run the super quick benchmark
Write-Host "`nRunning super quick benchmark..." -ForegroundColor Yellow
Write-Host "?? Configuration: 1 warmup, 2 measurements per test" -ForegroundColor Cyan
Write-Host "?? Testing only essential scenarios with minimal data.`n" -ForegroundColor Cyan

Write-Host "Running SuperQuickBenchmark..." -ForegroundColor Yellow
dotnet run -c Release --verbosity quiet -- super

Write-Host "`n?? Super quick benchmark completed!" -ForegroundColor Green
Write-Host "?? Results in BenchmarkDotNet.Artifacts folder" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? For more comprehensive testing:" -ForegroundColor Yellow
Write-Host "   ? Quick:  ./quick-test.ps1    (~2 minutes)" -ForegroundColor White
Write-Host "   ?? Full:   dotnet run -c Release -- all   (~10+ minutes)" -ForegroundColor White
Write-Host ""
Write-Host "?? Super quick optimizations:" -ForegroundColor Cyan
Write-Host "   - 1 warmup iteration" -ForegroundColor White
Write-Host "   - 2 measurement iterations only" -ForegroundColor White
Write-Host "   - 10 items max in collections" -ForegroundColor White
Write-Host "   - Memory diagnostics disabled" -ForegroundColor White
Write-Host "   - Results show relative performance only" -ForegroundColor White