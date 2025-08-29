# Facet Mapping Performance Benchmark Suite

This directory contains a comprehensive performance benchmarking suite that compares **Facet** mapping performance against two other popular .NET mapping libraries: **Mapster** and **Mapperly**.

## ?? Overview

The benchmark suite evaluates three distinct approaches to object mapping in .NET:

- **[Facet](https://github.com/efreeman518/Facet)** - Source generator-based mapping with compile-time code generation and EF Core integration
- **[Mapster](https://github.com/MapsterMapper/Mapster)** - Runtime mapping with expression tree compilation and flexible configuration
- **[Mapperly](https://github.com/riok/mapperly)** - Source generator-based mapping with compile-time code generation and explicit mapper interfaces

## ?? Project Structure

```
benchmark/
??? FacetBenchmark.sln              # Benchmark solution file
??? Facet.Benchmark/                # Main benchmark project
?   ??? Benchmarks/                 # Benchmark test classes
?   ?   ??? SingleEntityBenchmark.cs      # Individual object mapping tests
?   ?   ??? CollectionMappingBenchmark.cs # Bulk collection mapping tests
?   ?   ??? LinqProjectionBenchmark.cs    # IQueryable projection tests
?   ??? DTOs/
?   ?   ??? FacetDTOs.cs           # Facet-generated DTOs using [Facet] attribute
?   ?   ??? ManualDTOs.cs          # Manual DTOs for Mapster/Mapperly comparison
?   ??? Mappers/
?   ?   ??? MapperlyMappers.cs     # Mapperly source-generated mappers
?   ??? Models.cs                  # Domain entities for benchmarking
?   ??? TestDataGenerator.cs       # Reproducible test data generation
?   ??? Program.cs                 # Benchmark runner with interactive menu
?   ??? README.md                  # Detailed project documentation
??? README.md                      # This file
```

## ?? Quick Start

### Prerequisites

- **.NET 8.0 SDK** or later
- Windows, macOS, or Linux
- At least 4GB RAM (recommended for larger benchmarks)

### Running Benchmarks

1. **Navigate to the benchmark directory:**
   ```bash
   cd benchmark/Facet.Benchmark
   ```

2. **Build the project:**
   ```bash
   dotnet build -c Release
   ```

3. **Run interactive menu:**
   ```bash
   dotnet run -c Release
   ```

4. **Run specific benchmarks directly:**
   ```bash
   # Single entity mapping performance
   dotnet run -c Release -- single
   
   # Collection mapping performance
   dotnet run -c Release -- collection
   
   # LINQ projection performance
   dotnet run -c Release -- linq
   
   # Run all benchmarks
   dotnet run -c Release -- all
   ```

## ?? Benchmark Categories

### 1. Single Entity Mapping
Tests individual object transformation performance across:
- **User entities** ? Basic, Detailed, and Simple DTOs
- **Product entities** ? Full and Simple DTOs
- **Other entities** ? Address, Order, Category mappings

**Key Metrics:** Execution time, memory allocations, throughput

### 2. Collection Mapping
Tests bulk transformation performance with:
- **Collection sizes:** 10, 100, 1000 items
- **Entity types:** Users and Products
- **DTO variants:** Basic and Simple projections

**Key Metrics:** Scalability, memory efficiency, linear performance scaling

### 3. LINQ Projection
Tests expression tree generation and IQueryable performance:
- **Simple projections** ? Direct entity-to-DTO transformations
- **Filtered projections** ? Queries with WHERE clauses
- **Complex queries** ? Multi-level filtering, ordering, paging
- **Expression compilation** ? Overhead analysis

**Key Metrics:** IQueryable performance, expression compilation costs, EF Core simulation

## ?? Expected Performance Characteristics

Based on the design approaches, here's what you can generally expect:

### Facet (Source Generator)
- ? **Fastest execution** - Zero runtime overhead
- ? **Lowest memory usage** - No expression caching needed
- ? **EF Core optimized** - Built-in async extensions
- ? **Compile-time safety** - Errors caught at build time

### Mapperly (Source Generator)
- ? **Fast execution** - Compile-time generated code
- ? **Low memory usage** - No runtime compilation
- ? **Type safety** - Strong compile-time checking
- ?? **General purpose** - Not EF Core specialized

### Mapster (Runtime)
- ? **Highly flexible** - Runtime configuration
- ? **Feature rich** - Advanced mapping scenarios
- ?? **Expression compilation overhead** - First-run penalty
- ?? **Higher memory usage** - Expression caching

## ?? Interpreting Results

### Key Metrics Explained

- **Mean:** Average execution time per operation
- **Error:** Half of 99.9% confidence interval  
- **StdDev:** Standard deviation (consistency indicator)
- **Allocated:** Memory allocated per operation
- **Gen0/Gen1/Gen2:** Garbage collection pressure

### Performance Guidelines

| Metric | Excellent | Good | Acceptable | Needs Attention |
|--------|-----------|------|------------|-----------------|
| Single Entity | < 100ns | < 500ns | < 1?s | > 1?s |
| Collection (1000 items) | < 100?s | < 500?s | < 1ms | > 1ms |
| Memory per operation | < 100B | < 500B | < 1KB | > 1KB |

## ?? Customizing Benchmarks

### Adding New Entity Types

1. Add your entity to `Models.cs`
2. Create corresponding DTOs in `FacetDTOs.cs` (using `[Facet]`) and `ManualDTOs.cs`
3. Add Mapperly mappers in `MapperlyMappers.cs`
4. Configure Mapster mappings in benchmark setup methods
5. Add benchmark methods to appropriate benchmark classes

### Modifying Test Data

Edit `TestDataGenerator.cs` to:
- Change data generation patterns
- Adjust collection sizes
- Add new entity properties
- Modify data complexity

### Custom Benchmark Configuration

Modify `Program.cs` ? `CreateBenchmarkConfig()` to adjust:
- Number of warmup iterations
- Number of measurement iterations  
- Memory diagnostics settings
- Export formats (Markdown, HTML, JSON, CSV)

## ?? Output Formats

The benchmark suite generates multiple output formats:

1. **Console Output** - Real-time progress and results
2. **Markdown Reports** - GitHub-compatible tables and analysis
3. **HTML Reports** - Interactive web-based results viewer
4. **JSON Export** - Machine-readable data for analysis tools
5. **CSV Export** - Spreadsheet-compatible format
6. **Summary Report** - Comprehensive `BenchmarkSummary.md` with insights

## ?? Use Cases

### For Library Authors
- **Performance regression testing** during development
- **Optimization validation** after code changes
- **Competitive analysis** against other mapping solutions

### For Application Developers
- **Library selection guidance** based on your specific scenarios
- **Performance budget planning** for mapping operations
- **Bottleneck identification** in existing applications

### For Architecture Teams
- **Technology evaluation** for new projects
- **Performance standards definition** across teams
- **Best practices establishment** for mapping patterns

## ?? Contributing

We welcome contributions to improve the benchmark suite:

### Adding Benchmarks
- Create new benchmark methods following BenchmarkDotNet conventions
- Ensure reproducible test data using fixed random seeds
- Include comprehensive documentation for new scenarios

### Improving Analysis
- Enhance the report generation in `BenchmarkReportGenerator`
- Add new export formats or visualizations
- Improve performance analysis and recommendations

### Bug Reports
- Report issues with benchmark accuracy or consistency
- Suggest improvements to test data or scenarios
- Help identify platform-specific issues

## ?? Related Resources

- **[Facet Documentation](../docs/README.md)** - Complete Facet guide
- **[EF Core Extensions](../src/Facet.Extensions.EFCore/README.md)** - Async EF Core integration
- **[Custom Mapping](../docs/04_CustomMapping.md)** - Advanced mapping scenarios
- **[BenchmarkDotNet](https://benchmarkdotnet.org/)** - Benchmarking framework documentation

## ? Performance Tips

### For Accurate Benchmarking
- Always build in **Release** mode for measurements
- Ensure adequate **warmup iterations** for JIT compilation
- Use **realistic data sizes** that match your production scenarios
- Consider **memory allocation patterns**, not just execution time

### For Production Usage
- **Facet:** Leverage EF Core extensions for async database scenarios
- **Mapster:** Configure mappings once and reuse compiled expressions  
- **Mapperly:** Use explicit mapper interfaces for dependency injection

---

**Built with ?? for the .NET mapping performance community**

*Start your benchmarking journey with `dotnet run` and discover which mapping approach works best for your scenarios!*