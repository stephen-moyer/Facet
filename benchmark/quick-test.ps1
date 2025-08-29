#!/usr/bin/env pwsh

# Quick benchmark runner script  
# This runs optimized benchmarks for faster comparison testing

Write-Host "? Quick Facet Benchmark Test" -ForegroundColor Green
Write-Host "==============================" 
Write-Host "Running optimized benchmarks for faster comparison" -ForegroundColor Cyan
Write-Host ""

# Build in Release mode
Write-Host "Building in Release mode..." -ForegroundColor Yellow
dotnet build -c Release --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "? Build successful!" -ForegroundColor Green

# Run the quick comparison benchmark  
Write-Host "`nRunning quick comparison benchmark..." -ForegroundColor Yellow
Write-Host "?? Configuration: 1 warmup iteration, 5 measurement iterations" -ForegroundColor Cyan
Write-Host "?? Testing key scenarios with small datasets (25 items).`n" -ForegroundColor Cyan

Write-Host "Running QuickComparisonBenchmark..." -ForegroundColor Yellow
dotnet run -c Release -- quick

Write-Host "`n? Quick benchmark test completed!" -ForegroundColor Green
Write-Host "?? Check the BenchmarkDotNet.Artifacts folder for detailed results" -ForegroundColor Cyan
Write-Host "?? Look for .md, .html, .json, and .csv files in the results directory" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? For even faster testing:" -ForegroundColor Yellow
Write-Host "   ?? Super Quick: ./super-quick-test.ps1    (~30 seconds)" -ForegroundColor White
Write-Host ""
Write-Host "?? For comprehensive benchmarks:" -ForegroundColor Yellow
Write-Host "   ?? Full Suite: dotnet run -c Release -- all   (~10+ minutes)" -ForegroundColor White
Write-Host ""
Write-Host "?? Quick benchmark optimizations:" -ForegroundColor Cyan
Write-Host "   - 1 warmup iteration (vs 3 in full benchmarks)" -ForegroundColor White
Write-Host "   - 5 measurement iterations (vs 10 in full benchmarks)" -ForegroundColor White
Write-Host "   - Small datasets (25 items vs 1000+ in full benchmarks)" -ForegroundColor White
Write-Host "   - Results sufficient for performance comparison" -ForegroundColor White
Write-Host ""
Write-Host "?? Expected completion time: ~2 minutes" -ForegroundColor Green