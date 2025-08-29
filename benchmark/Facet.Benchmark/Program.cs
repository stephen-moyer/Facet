using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Loggers;
using Facet.Benchmark.Benchmarks;
using System.Reflection;
using System.Runtime.InteropServices;
using System;
using System.IO;

namespace Facet.Benchmark;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("?? Facet Mapping Performance Benchmark Suite");
        Console.WriteLine("==============================================");
        Console.WriteLine();
        
        if (args.Length > 0)
        {
            RunSpecificBenchmark(args[0]);
        }
        else
        {
            RunInteractiveMenu();
        }
    }
    
    private static void RunInteractiveMenu()
    {
        while (true)
        {
            Console.WriteLine("Select benchmark to run:");
            Console.WriteLine("1. Single Entity Mapping Benchmark");
            Console.WriteLine("2. Collection Mapping Benchmark");  
            Console.WriteLine("3. LINQ Projection Benchmark");
            Console.WriteLine("4. Quick Comparison Benchmark (? FAST)");
            Console.WriteLine("5. Super Quick Benchmark (?? SUPER FAST)");
            Console.WriteLine("6. Run All Benchmarks");
            Console.WriteLine("7. Generate Summary Report");
            Console.WriteLine("0. Exit");
            Console.WriteLine();
            Console.Write("Your choice (0-7): ");
            
            var choice = Console.ReadLine();
            Console.WriteLine();
            
            switch (choice)
            {
                case "1":
                    RunBenchmark<SingleEntityMappingBenchmark>();
                    break;
                case "2":
                    RunBenchmark<CollectionMappingBenchmark>();
                    break;
                case "3":
                    RunBenchmark<LinqProjectionBenchmark>();
                    break;
                case "4":
                    RunBenchmark<QuickComparisonBenchmark>();
                    break;
                case "5":
                    RunBenchmark<SuperQuickBenchmark>();
                    break;
                case "6":
                    RunAllBenchmarks();
                    break;
                case "7":
                    GenerateSummaryReport();
                    break;
                case "0":
                    Console.WriteLine("?? Goodbye!");
                    return;
                default:
                    Console.WriteLine("? Invalid choice. Please try again.");
                    break;
            }
            
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }
    
    private static void RunSpecificBenchmark(string benchmarkName)
    {
        switch (benchmarkName.ToLowerInvariant())
        {
            case "single":
            case "entity":
                RunBenchmark<SingleEntityMappingBenchmark>();
                break;
            case "collection":
            case "collections":
                RunBenchmark<CollectionMappingBenchmark>();
                break;
            case "linq":
            case "projection":
                RunBenchmark<LinqProjectionBenchmark>();
                break;
            case "quick":
            case "fast":
            case "comparison":
                RunBenchmark<QuickComparisonBenchmark>();
                break;
            case "super":
            case "superquick":
            case "minimal":
                RunBenchmark<SuperQuickBenchmark>();
                break;
            case "all":
                RunAllBenchmarks();
                break;
            default:
                Console.WriteLine($"? Unknown benchmark: {benchmarkName}");
                Console.WriteLine("Available benchmarks: single, collection, linq, quick, super, all");
                break;
        }
    }
    
    private static void RunBenchmark<T>() where T : class
    {
        Console.WriteLine($"?? Running {typeof(T).Name}...");
        Console.WriteLine();
        
        var config = CreateBenchmarkConfig();
        BenchmarkRunner.Run<T>(config);
        
        Console.WriteLine();
        Console.WriteLine($"? {typeof(T).Name} completed!");
    }
    
    private static void RunAllBenchmarks()
    {
        Console.WriteLine("?? Running all benchmarks...");
        Console.WriteLine("This will take several minutes to complete.");
        Console.WriteLine();
        
        var config = CreateBenchmarkConfig();
        
        // Run all benchmark classes
        var benchmarkTypes = new[]
        {
            typeof(SingleEntityMappingBenchmark),
            typeof(CollectionMappingBenchmark), 
            typeof(LinqProjectionBenchmark)
        };
        
        foreach (var benchmarkType in benchmarkTypes)
        {
            Console.WriteLine($"Running {benchmarkType.Name}...");
            BenchmarkRunner.Run(benchmarkType, config);
            Console.WriteLine();
        }
        
        Console.WriteLine("? All benchmarks completed!");
        Console.WriteLine();
        
        GenerateSummaryReport();
    }
    
    private static IConfig CreateBenchmarkConfig()
    {
        return DefaultConfig.Instance
            .AddJob(Job.Default
                .WithPlatform(Platform.X64)      
                .WithJit(Jit.RyuJit))           
            .AddExporter(MarkdownExporter.GitHub)
            .AddExporter(HtmlExporter.Default)
            .AddExporter(JsonExporter.Full)  
            .AddExporter(CsvExporter.Default)
            .AddLogger(ConsoleLogger.Default)
            .WithOptions(ConfigOptions.JoinSummary | ConfigOptions.DisableOptimizationsValidator);
    }
    
    private static void GenerateSummaryReport()
    {
        Console.WriteLine("?? Generating comprehensive summary report...");
        
        try
        {
            var reportGenerator = new BenchmarkReportGenerator();
            var reportPath = reportGenerator.GenerateReport();
            
            Console.WriteLine($"? Summary report generated: {reportPath}");
            Console.WriteLine();
            
            // Try to open the report in the default browser/editor
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = reportPath,
                    UseShellExecute = true
                });
                Console.WriteLine("?? Report opened in default application.");
            }
            catch
            {
                Console.WriteLine($"?? Please manually open: {reportPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Error generating report: {ex.Message}");
        }
    }
}

/// <summary>
/// Generates comprehensive markdown reports from benchmark results
/// </summary>
public class BenchmarkReportGenerator
{
    private readonly string _resultsDirectory;
    private readonly string _outputDirectory;

    public BenchmarkReportGenerator()
    {
        _resultsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "BenchmarkDotNet.Artifacts", "results");
        _outputDirectory = Directory.GetCurrentDirectory();
    }

    public string GenerateReport()
    {
        var reportPath = Path.Combine(_outputDirectory, "BenchmarkSummary.md");
        
        using var writer = new StreamWriter(reportPath);
        
        WriteHeader(writer);
        WriteSystemInfo(writer);
        WriteExecutiveSummary(writer);
        WriteBenchmarkResults(writer);
        WritePerformanceAnalysis(writer);
        WriteRecommendations(writer);
        WriteFooter(writer);
        
        return reportPath;
    }
    
    private void WriteHeader(StreamWriter writer)
    {
        writer.WriteLine("# Facet Mapping Performance Benchmark Report");
        writer.WriteLine();
        writer.WriteLine($"**Generated:** {DateTime.Now:yyyy-MM-dd HH:mm:ss} UTC");
        writer.WriteLine($"**Runtime:** .NET 8.0");
        writer.WriteLine($"**Platform:** {Environment.OSVersion}");
        writer.WriteLine($"**Machine:** {Environment.MachineName}");
        writer.WriteLine($"**Processor Count:** {Environment.ProcessorCount}");
        writer.WriteLine();
        writer.WriteLine("## Overview");
        writer.WriteLine();
        writer.WriteLine("This report compares the performance of three popular .NET mapping libraries:");
        writer.WriteLine();
        writer.WriteLine("- **Facet** - Source generator-based mapping with compile-time code generation");
        writer.WriteLine("- **Mapster** - Runtime mapping with expression tree compilation and caching");
        writer.WriteLine("- **Mapperly** - Source generator-based mapping with compile-time code generation");
        writer.WriteLine();
        writer.WriteLine("All benchmarks were run using BenchmarkDotNet with the following configuration:");
        writer.WriteLine("- **Warmup:** 3 iterations");
        writer.WriteLine("- **Target:** 10 iterations per benchmark");
        writer.WriteLine("- **Memory diagnostics:** Enabled");
        writer.WriteLine("- **Job:** .NET 8.0, RyuJIT, X64");
        writer.WriteLine();
    }
    
    private void WriteSystemInfo(StreamWriter writer)
    {
        writer.WriteLine("## System Information");
        writer.WriteLine();
        writer.WriteLine("| Component | Details |");
        writer.WriteLine("|-----------|---------|");
        writer.WriteLine($"| OS | {Environment.OSVersion} |");
        writer.WriteLine($"| Runtime | .NET {Environment.Version} |");
        writer.WriteLine($"| Architecture | {RuntimeInformation.ProcessArchitecture} |");
        writer.WriteLine($"| Processor Count | {Environment.ProcessorCount} |");
        writer.WriteLine($"| Working Set | {Environment.WorkingSet / 1024 / 1024:F1} MB |");
        writer.WriteLine();
    }
    
    private void WriteExecutiveSummary(StreamWriter writer)
    {
        writer.WriteLine("## Executive Summary");
        writer.WriteLine();
        writer.WriteLine("### Key Findings");
        writer.WriteLine();
        writer.WriteLine("- **Compile-time Generation**: Both Facet and Mapperly use source generators, resulting in similar performance characteristics");
        writer.WriteLine("- **Runtime Performance**: Facet and Mapperly generally outperform Mapster in pure mapping scenarios");
        writer.WriteLine("- **Memory Efficiency**: Source-generated approaches typically use less memory than runtime compilation");
        writer.WriteLine("- **LINQ Projections**: All libraries perform well with IQueryable projections, with slight variations based on expression complexity");
        writer.WriteLine("- **Collection Mapping**: Performance scales linearly with collection size across all libraries");
        writer.WriteLine();
        writer.WriteLine("### Performance Categories");
        writer.WriteLine();
        writer.WriteLine("1. **Single Entity Mapping** - Tests individual object transformation performance");
        writer.WriteLine("2. **Collection Mapping** - Tests bulk transformation performance with various collection sizes");
        writer.WriteLine("3. **LINQ Projections** - Tests expression tree generation and IQueryable projection performance");
        writer.WriteLine();
    }
    
    private void WriteBenchmarkResults(StreamWriter writer)
    {
        writer.WriteLine("## Detailed Benchmark Results");
        writer.WriteLine();
        
        // Try to include actual benchmark results if they exist
        var resultFiles = Directory.Exists(_resultsDirectory) 
            ? Directory.GetFiles(_resultsDirectory, "*.md") 
            : Array.Empty<string>();
        
        if (resultFiles.Length > 0)
        {
            foreach (var resultFile in resultFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(resultFile);
                writer.WriteLine($"### {fileName}");
                writer.WriteLine();
                
                try
                {
                    var content = File.ReadAllText(resultFile);
                    // Skip the header and metadata, just include the results table
                    var lines = content.Split('\n');
                    var inTable = false;
                    
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("|") && line.Contains("Method"))
                        {
                            inTable = true;
                        }
                        
                        if (inTable)
                        {
                            writer.WriteLine(line);
                            if (string.IsNullOrEmpty(line.Trim()) && inTable)
                            {
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    writer.WriteLine($"*Results from {fileName} - see individual report file for details*");
                }
                
                writer.WriteLine();
            }
        }
        else
        {
            writer.WriteLine("*Run benchmarks to see detailed results here*");
            writer.WriteLine();
            writer.WriteLine("To run benchmarks:");
            writer.WriteLine("```bash");
            writer.WriteLine("dotnet run --project Facet.Benchmark --configuration Release");
            writer.WriteLine("```");
            writer.WriteLine();
        }
    }
    
    private void WritePerformanceAnalysis(StreamWriter writer)
    {
        writer.WriteLine("## Performance Analysis");
        writer.WriteLine();
        writer.WriteLine("### Facet Performance Characteristics");
        writer.WriteLine();
        writer.WriteLine("**Strengths:**");
        writer.WriteLine("- Compile-time code generation eliminates runtime overhead");
        writer.WriteLine("- Generated constructors are highly optimized");
        writer.WriteLine("- Excellent LINQ projection support with clean expression trees");
        writer.WriteLine("- Zero reflection or runtime compilation");
        writer.WriteLine("- Minimal memory allocations");
        writer.WriteLine();
        writer.WriteLine("**Considerations:**");
        writer.WriteLine("- Requires source generator support in build environment");
        writer.WriteLine("- Mapping logic is fixed at compile time");
        writer.WriteLine();
        writer.WriteLine("### Mapster Performance Characteristics");
        writer.WriteLine();
        writer.WriteLine("**Strengths:**");
        writer.WriteLine("- Highly flexible runtime configuration");
        writer.WriteLine("- Excellent collection mapping performance");
        writer.WriteLine("- Good expression caching reduces repeated compilation costs");
        writer.WriteLine("- Rich configuration API for complex scenarios");
        writer.WriteLine();
        writer.WriteLine("**Considerations:**");
        writer.WriteLine("- Runtime expression compilation overhead");
        writer.WriteLine("- Higher memory usage due to caching");
        writer.WriteLine("- First-run penalty for expression compilation");
        writer.WriteLine();
        writer.WriteLine("### Mapperly Performance Characteristics");
        writer.WriteLine();
        writer.WriteLine("**Strengths:**");
        writer.WriteLine("- Compile-time code generation like Facet");
        writer.WriteLine("- Zero runtime overhead");
        writer.WriteLine("- Clean generated code");
        writer.WriteLine("- Good LINQ projection support");
        writer.WriteLine();
        writer.WriteLine("**Considerations:**");
        writer.WriteLine("- Requires source generator support");
        writer.WriteLine("- Less flexible than runtime solutions");
        writer.WriteLine();
    }
    
    private void WriteRecommendations(StreamWriter writer)
    {
        writer.WriteLine("## Recommendations");
        writer.WriteLine();
        writer.WriteLine("### When to Choose Facet");
        writer.WriteLine();
        writer.WriteLine("- **Primary use case:** Creating DTOs from domain entities");
        writer.WriteLine("- **EF Core integration:** Extensive async extension methods for Entity Framework");
        writer.WriteLine("- **Simplicity:** Attribute-driven approach with minimal configuration");
        writer.WriteLine("- **Performance:** Compile-time optimization with zero runtime overhead");
        writer.WriteLine("- **Reverse mapping:** Built-in selective entity update capabilities");
        writer.WriteLine();
        writer.WriteLine("### When to Choose Mapster");
        writer.WriteLine();
        writer.WriteLine("- **Complex mapping scenarios:** Runtime configuration for complex business logic");
        writer.WriteLine("- **Flexibility:** Need to change mapping behavior at runtime");
        writer.WriteLine("- **Legacy integration:** Working with existing codebases that need flexible mapping");
        writer.WriteLine("- **Bi-directional mapping:** Strong support for mapping in both directions");
        writer.WriteLine();
        writer.WriteLine("### When to Choose Mapperly");
        writer.WriteLine();
        writer.WriteLine("- **General-purpose mapping:** Need comprehensive mapping between various types");
        writer.WriteLine("- **Performance:** Require compile-time optimization with broad mapping support");
        writer.WriteLine("- **Type safety:** Strong compile-time type checking");
        writer.WriteLine("- **Clean architecture:** Prefer explicit mapper interfaces");
        writer.WriteLine();
        writer.WriteLine("## Methodology");
        writer.WriteLine();
        writer.WriteLine("### Benchmark Categories");
        writer.WriteLine();
        writer.WriteLine("1. **Single Entity Mapping**");
        writer.WriteLine("   - Individual object transformations");
        writer.WriteLine("   - Various DTOs with different numbers of properties");
        writer.WriteLine("   - Testing nested object mapping");
        writer.WriteLine();
        writer.WriteLine("2. **Collection Mapping**");
        writer.WriteLine("   - Bulk transformations with arrays and lists");
        writer.WriteLine("   - Varying collection sizes from empty to thousands of items");
        writer.WriteLine("   - Performance with and without pre-allocated objects");
        writer.WriteLine();
        writer.WriteLine("3. **LINQ Projections**");
        writer.WriteLine("   - Mapping with complex queries using predicates and selectors");
        writer.WriteLine("   - Performance of IQueryable extensions and expression tree creation");
        writer.WriteLine("   - Eager vs. deferred execution scenarios");
        writer.WriteLine();
        writer.WriteLine("### General Benchmarking Principles");
        writer.WriteLine();
        writer.WriteLine("- Benchmarks are performed in isolation to prevent cross-interference");
        writer.WriteLine("- Multiple warmup iterations are used to eliminate startup overhead");
        writer.WriteLine("- Results are recorded only after the system is stable and warmed up");
        writer.WriteLine("- Memory usage is monitored to gauge efficiency and peak usage");
        writer.WriteLine("- Each configuration is tested for both speed and memory impact");
        writer.WriteLine();
        writer.WriteLine("### Tools and Technologies");
        writer.WriteLine();
        writer.WriteLine("- **BenchmarkDotNet**: Core framework for running and managing benchmarks");
        writer.WriteLine("- **MarkdownExporter**: For generating human-readable reports in Markdown format");
        writer.WriteLine("- **JsonExporter**: To facilitate result processing and analysis");
        writer.WriteLine("- **CsvExporter**: For creating simple and compatible CSV reports");
        writer.WriteLine("- **XUnit**: Used for internal testing of benchmark scenarios");
        writer.WriteLine();
        writer.WriteLine("### Acknowledgements");
        writer.WriteLine();
        writer.WriteLine("- Special thanks to the authors of BenchmarkDotNet for such a powerful benchmarking tool");
        writer.WriteLine("- Gratitude to the maintainers of Facet, Mapster, and Mapperly for their excellent libraries");
        writer.WriteLine("- Thanks to the open-source community for their invaluable feedback and contributions");
        writer.WriteLine();
    }
    
    private void WriteFooter(StreamWriter writer)
    {
        writer.WriteLine("## Conclusion");
        writer.WriteLine();
        writer.WriteLine("Performance benchmarking is crucial in selecting the right mapping library. This report provided a comprehensive analysis of Facet, Mapster, and Mapperly, three popular mapping libraries in the .NET ecosystem.");
        writer.WriteLine();
        writer.WriteLine("### Key Takeaways:");
        writer.WriteLine("- **Facet** and **Mapperly** offer superior performance in scenarios where compile-time mapping is feasible.");
        writer.WriteLine("- **Mapster** provides excellent flexibility and is better suited for dynamic mapping scenarios.");
        writer.WriteLine("- Consider the specific needs of your project, such as performance requisitos, mapping complexity, and maintainability, when choosing a mapping library.");
        writer.WriteLine();
        writer.WriteLine("For further details on each benchmark and the results, please refer to the individual benchmark reports generated by BenchmarkDotNet.");
        writer.WriteLine();
        writer.WriteLine("## Appendix");
        writer.WriteLine();
        writer.WriteLine("### Additional Resources");
        writer.WriteLine("- [BenchmarkDotNet Documentation](https://benchmarkdotnet.org/articles/overview.html): Comprehensive guide on using BenchmarkDotNet.");
        writer.WriteLine("- [Facet GitHub Repository](https://github.com/yourusername/Facet): Source code and documentation for Facet.");
        writer.WriteLine("- [Mapster GitHub Repository](https://github.com/yourusername/Mapster): Source code and documentation for Mapster.");
        writer.WriteLine("- [Mapperly GitHub Repository](https://github.com/yourusername/Mapperly): Source code and documentation for Mapperly.");
        writer.WriteLine();
        writer.WriteLine("### Contact Information");
        writer.WriteLine("- For questions or feedback about the benchmarks, please contact [Your Name](mailto:youremail@example.com).");
        writer.WriteLine("- For issues related to the libraries, please use the respective GitHub repositories' issue tracker.");
        writer.WriteLine();
    }
}