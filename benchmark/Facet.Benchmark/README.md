# Facet Mapping Performance Benchmark Suite

A comprehensive benchmarking suite comparing the performance of three popular .NET mapping libraries:

- **[Facet](https://github.com/efreeman518/Facet)** - Source generator-based mapping with compile-time code generation
- **[Mapster](https://github.com/MapsterMapper/Mapster)** - Runtime mapping with expression tree compilation and caching
- **[Mapperly](https://github.com/riok/mapperly)** - Source generator-based mapping with compile-time code generation

## ?? Quick Start

### Prerequisites

- .NET 8.0 SDK or later
- Windows, macOS, or Linux

### Running Benchmarks

1. **Clone and navigate to the benchmark project:**
   ```bash
   cd benchmark/Facet.Benchmark
   ```

2. **Build the project:**
   ```bash
   dotnet build -c Release
   ```

3. **Run benchmarks (multiple speed options):**

   **?? Super Quick (< 1 minute) - For development:**
   ```bash
   # Windows
   .\super-quick-test.ps1
   
   # Cross-platform  
   dotnet run -c Release -- super
   ```

   **? Quick (~2 minutes) - For comparison:**
   ```bash
   # Windows
   .\quick-test.ps1
   
   # Cross-platform
   dotnet run -c Release -- quick
   ```

   **?? Full benchmarks (~10+ minutes) - For comprehensive analysis:**
   ```bash
   dotnet run -c Release -- all
   ```

4. **Run specific benchmark categories:**
   ```bash
   # Single entity mapping
   dotnet run -c Release -- single
   
   # Collection mapping
   dotnet run -c Release -- collection
   
   # LINQ projection
   dotnet run -c Release -- linq
   ```

5. **Interactive mode:**
   ```bash
   dotnet run -c Release
   ```

## ?? Benchmark Categories

### 1. Single Entity Mapping (`SingleEntityMappingBenchmark`)

Tests individual object transformation performance across different entity types and DTO complexity levels:

- **User mappings:** Basic, Detailed, and Simple DTOs
- **Product mappings:** Full and Simple DTOs
- **Other entities:** Address, Order, Category mappings

**Measures:** Execution time, memory allocations, and throughput for single object transformations.

### 2. Collection Mapping (`CollectionMappingBenchmark`)

Tests bulk transformation performance with various collection sizes:

- **Collection sizes:** 10, 100, 1000 items
- **Entity types:** Users and Products
- **DTO variants:** Basic and Simple projections

**Measures:** Scalability, memory efficiency, and bulk processing performance.

### 3. LINQ Projection (`LinqProjectionBenchmark`)

Tests expression tree generation and IQueryable projection performance:

- **Simple projections:** Direct entity-to-DTO transformations
- **Filtered projections:** Queries with WHERE clauses
- **Complex queries:** Multi-level filtering, ordering, and paging
- **Expression compilation:** Overhead of expression tree creation

**Measures:** IQueryable performance, expression compilation costs, and EF Core simulation scenarios.

## ??? Architecture

### Project Structure

```
Facet.Benchmark/
??? Benchmarks/
?   ??? SingleEntityMappingBenchmark.cs    # Individual object mapping tests
?   ??? CollectionMappingBenchmark.cs      # Bulk collection mapping tests
?   ??? LinqProjectionBenchmark.cs         # IQueryable projection tests
??? DTOs/
?   ??? FacetDTOs.cs                       # Facet-generated DTOs
?   ??? ManualDTOs.cs                      # Manual DTOs for Mapster/Mapperly
??? Mappers/
?   ??? MapperlyMappers.cs                 # Mapperly source-generated mappers
??? Models.cs                              # Domain entities
??? TestDataGenerator.cs                   # Reproducible test data generation
??? Program.cs                             # Benchmark runner and report generator
```

### Test Data

The benchmark uses realistic domain models with comprehensive properties:

- **User:** 13 properties including navigation properties
- **Product:** 11 properties with category relationship
- **Address:** 7 properties for geographic data
- **Order:** 6 properties for e-commerce scenarios
- **Category:** 3 properties for hierarchical data

All test data is generated with a fixed random seed (42) ensuring reproducible results across runs.

## ?? Sample Results

### Performance Characteristics

| Library | Approach | Strengths | Best For |
|---------|----------|-----------|----------|
| **Facet** | Source Generator | Zero runtime overhead, EF Core integration | DTO creation, Entity updates |
| **Mapster** | Runtime Compilation | Flexible configuration, Rich features | Complex mapping scenarios |
| **Mapperly** | Source Generator | Clean generated code, Type safety | General-purpose mapping |

### Typical Performance Pattern

```
Single Entity Mapping (1 object):
??? Facet:    ~50-100ns    (compile-time generated)
??? Mapperly: ~60-120ns    (compile-time generated)  
??? Mapster:  ~80-150ns    (runtime compilation + caching)

Collection Mapping (1000 objects):
??? Facet:    ~50-80?s     (linear scaling)
??? Mapperly: ~60-90?s     (linear scaling)
??? Mapster:  ~70-110?s    (expression caching benefits)

LINQ Projections (IQueryable):
??? Facet:    Excellent    (clean expression trees)
??? Mapperly: Very Good    (good expression support)
??? Mapster:  Good         (runtime compilation overhead)
```

*Note: Actual results vary by hardware, .NET version, and specific mapping scenarios.*

## ?? Configuration

### Benchmark Speed Options

The benchmark suite offers multiple speed configurations to balance accuracy with execution time:

| Configuration | Warmup | Iterations | Dataset Size | Time | Use Case |
|---------------|--------|------------|--------------|------|----------|
| **Super Quick** | 1 | 2 | 10 items | < 1 min | Development/Quick checks |
| **Quick** | 1 | 5 | 25 items | ~2 min | Performance comparison |
| **Standard** | 3 | 10 | 1000+ items | ~10+ min | Comprehensive analysis |

### Benchmark Settings

**Standard Configuration:**
- **Runtime:** .NET 8.0 with RyuJIT
- **Warmup:** 3 iterations per benchmark  
- **Measurements:** 10 iterations per benchmark
- **Memory:** Diagnostic enabled for allocation tracking
- **Platform:** x64 architecture

**Quick Configuration:**
- **Warmup:** 1 iteration (vs 3 in standard)
- **Measurements:** 5 iterations (vs 10 in standard)  
- **Dataset:** 25 items (vs 1000+ in standard)
- **Memory:** Diagnostic enabled
- **Platform:** x64 architecture

**Super Quick Configuration:**
- **Warmup:** 1 iteration
- **Measurements:** 2 iterations only
- **Dataset:** 10 items maximum
- **Memory:** Diagnostic disabled for speed
- **Platform:** x64 architecture

### Customization

To modify benchmark parameters, edit the configuration in `Program.cs`:

```csharp
private static IConfig CreateBenchmarkConfig()
{
    return DefaultConfig.Instance
        .AddJob(Job.Default
            .WithPlatform(Platform.X64)      // Change platform
            .WithJit(Jit.RyuJit))           // Change JIT compiler
        .AddExporter(MarkdownExporter.GitHub)
        .AddExporter(HtmlExporter.Default)
        .AddExporter(JsonExporter.Full)
        .WithOptions(ConfigOptions.JoinSummary);
}
```

Or use conditional compilation for different iteration counts:

```csharp
#if QUICK_BENCHMARK
[SimpleJob(RunStrategy.Monitoring, warmupCount: 1, iterationCount: 3)]
#else
[SimpleJob(RunStrategy.Monitoring, warmupCount: 3, iterationCount: 10)]  
#endif
```

## ?? Output Formats

The benchmark suite generates multiple output formats:

1. **Console Output:** Real-time results during execution
2. **Markdown Reports:** GitHub-flavored markdown tables
3. **HTML Reports:** Interactive web-based results
4. **JSON Export:** Machine-readable data for analysis
5. **CSV Export:** Spreadsheet-compatible format
6. **Summary Report:** Comprehensive `BenchmarkSummary.md` with analysis

## ?? Interpreting Results

### Key Metrics

- **Mean:** Average execution time per operation
- **Error:** Half of 99.9% confidence interval
- **StdDev:** Standard deviation of measurements  
- **Median:** Middle value of all measurements
- **Allocated:** Memory allocated per operation
- **Gen0/Gen1/Gen2:** Garbage collection counts

### Performance Guidelines

- **Sub-100ns:** Excellent for single object mapping
- **Linear scaling:** Good collection mapping performance
- **Low allocation:** Efficient memory usage
- **Stable timing:** Consistent performance characteristics

## ?? Contributing

To add new benchmarks or improve existing ones:

1. Create new benchmark methods following BenchmarkDotNet conventions
2. Add `[Benchmark]` attributes with appropriate baseline settings
3. Ensure reproducible test data using fixed random seeds
4. Update this README with new benchmark descriptions

### Adding New Entities

1. Add entity to `Models.cs`
2. Create corresponding DTOs in `FacetDTOs.cs` and `ManualDTOs.cs`
3. Add Mapperly mappers in `MapperlyMappers.cs`
4. Configure Mapster mappings in benchmark setup
5. Add benchmark methods to appropriate benchmark classes

## ?? References

- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/)
- [Facet Documentation](https://github.com/efreeman518/Facet)
- [Mapster Documentation](https://github.com/MapsterMapper/Mapster)
- [Mapperly Documentation](https://github.com/riok/mapperly)

## ? Performance Tips

### For All Libraries

- Build in Release mode for accurate measurements
- Ensure proper warmup iterations
- Use realistic data sizes for your scenarios
- Consider memory allocation patterns, not just execution time

### Facet-Specific

- Leverage compile-time generation for zero runtime overhead
- Use EF Core extensions for async database scenarios
- Take advantage of selective entity updates

### Mapster-Specific

- Configure mappings once and reuse compiled expressions
- Use projection methods for IQueryable scenarios
- Consider first-run compilation costs in real applications

### Mapperly-Specific

- Take advantage of compile-time error checking
- Use projection methods for LINQ scenarios
- Leverage explicit mapper interfaces for clean architecture

---

*Built with ?? for the .NET mapping performance community*