#!/usr/bin/env pwsh

# Quick benchmark runner script
# This runs a subset of benchmarks for quick testing

Write-Host "?? Quick Facet Benchmark Test" -ForegroundColor Green
Write-Host "==============================" 

# Build in release mode
Write-Host "Building in Release mode..." -ForegroundColor Yellow
dotnet build -c Release --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "? Build successful!" -ForegroundColor Green

# Run a quick single benchmark to test
Write-Host "`nRunning quick benchmark test..." -ForegroundColor Yellow
Write-Host "This will run only a few methods for faster results.`n" -ForegroundColor Cyan

# Create a temporary benchmark config with fewer iterations
$env:BENCHMARK_FILTER = "*FacetUserBasic*|*MapsterUserBasic*|*MapperlyUserBasic*"

dotnet run -c Release -- single

Write-Host "`n? Quick benchmark test completed!" -ForegroundColor Green
Write-Host "?? Check the BenchmarkDotNet.Artifacts folder for detailed results" -ForegroundColor Cyan
Write-Host "?? Look for .md, .html, .json, and .csv files in the results directory" -ForegroundColor Cyan