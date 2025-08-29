using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Facet.Benchmark.Models;
using Facet.Benchmark.DTOs;
using Facet.Benchmark.Mappers;
using Facet.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Facet.Benchmark.Benchmarks;

/// <summary>
/// Benchmarks LINQ projection performance simulating EF Core scenarios
/// This tests Expression tree generation and IQueryable projections
/// </summary>
#if QUICK_BENCHMARK
[SimpleJob(RunStrategy.Monitoring, warmupCount: 1, iterationCount: 3)]
#else
[SimpleJob(RunStrategy.Monitoring, warmupCount: 3, iterationCount: 10)]
#endif
[MemoryDiagnoser]
[MarkdownExporter]
[JsonExporter]
[HtmlExporter]
public class LinqProjectionBenchmark
{
    private IQueryable<User> _userQueryable = null!;
    private IQueryable<Product> _productQueryable = null!;
    
    private UserMapper _userMapper = null!;
    private ProductMapper _productMapper = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Generate test data
        var users = TestDataGenerator.GenerateUsers(1000);
        var products = TestDataGenerator.GenerateProducts(500);
        
        // Create in-memory queryables to simulate EF Core scenarios
        _userQueryable = users.AsQueryable();
        _productQueryable = products.AsQueryable();
        
        _userMapper = new UserMapper();
        _productMapper = new ProductMapper();
        
        // Configure Mapster for projections
        ConfigureMapster();
    }

    private static void ConfigureMapster()
    {
        // Configure Mapster mappings - just use default mappings
        TypeAdapterConfig<User, UserBasicManualDto>.NewConfig().Compile();
        TypeAdapterConfig<User, UserSimpleManualDto>.NewConfig().Compile();
        TypeAdapterConfig<Product, ProductManualDto>.NewConfig().Compile();
        TypeAdapterConfig<Product, ProductSimpleManualDto>.NewConfig().Compile();
    }

    #region User Projection Benchmarks

    [Benchmark(Baseline = true)]
    public List<UserBasicDto> FacetUserProjection()
    {
        return _userQueryable
            .SelectFacet<User, UserBasicDto>()
            .ToList();
    }

    [Benchmark]
    public List<UserBasicManualDto> MapsterUserProjection()
    {
        return _userQueryable
            .ProjectToType<UserBasicManualDto>()
            .ToList();
    }

    [Benchmark]
    public List<UserBasicManualDto> MapperlyUserProjection()
    {
        return _userMapper
            .ProjectToBasicDto(_userQueryable)
            .ToList();
    }

    [Benchmark]
    public List<UserBasicDto> FacetUserProjectionWithFilter()
    {
        return _userQueryable
            .Where(u => u.IsActive && u.LoginCount > 10)
            .SelectFacet<User, UserBasicDto>()
            .ToList();
    }

    [Benchmark]
    public List<UserBasicManualDto> MapsterUserProjectionWithFilter()
    {
        return _userQueryable
            .Where(u => u.IsActive && u.LoginCount > 10)
            .ProjectToType<UserBasicManualDto>()
            .ToList();
    }

    [Benchmark]
    public List<UserBasicManualDto> MapperlyUserProjectionWithFilter()
    {
        return _userMapper
            .ProjectToBasicDto(_userQueryable.Where(u => u.IsActive && u.LoginCount > 10))
            .ToList();
    }

    [Benchmark]
    public List<UserSimpleDto> FacetUserSimpleProjection()
    {
        return _userQueryable
            .SelectFacet<User, UserSimpleDto>()
            .ToList();
    }

    [Benchmark]
    public List<UserSimpleManualDto> MapsterUserSimpleProjection()
    {
        return _userQueryable
            .ProjectToType<UserSimpleManualDto>()
            .ToList();
    }

    [Benchmark]
    public List<UserSimpleManualDto> MapperlyUserSimpleProjection()
    {
        return _userMapper
            .ProjectToSimpleDto(_userQueryable)
            .ToList();
    }

    #endregion

    #region Product Projection Benchmarks

    [Benchmark]
    public List<ProductDto> FacetProductProjection()
    {
        return _productQueryable
            .SelectFacet<Product, ProductDto>()
            .ToList();
    }

    [Benchmark]
    public List<ProductManualDto> MapsterProductProjection()
    {
        return _productQueryable
            .ProjectToType<ProductManualDto>()
            .ToList();
    }

    [Benchmark]
    public List<ProductManualDto> MapperlyProductProjection()
    {
        return _productMapper
            .ProjectToDto(_productQueryable)
            .ToList();
    }

    [Benchmark]
    public List<ProductSimpleDto> FacetProductSimpleProjection()
    {
        return _productQueryable
            .SelectFacet<Product, ProductSimpleDto>()
            .ToList();
    }

    [Benchmark]
    public List<ProductSimpleManualDto> MapsterProductSimpleProjection()
    {
        return _productQueryable
            .ProjectToType<ProductSimpleManualDto>()
            .ToList();
    }

    [Benchmark]
    public List<ProductSimpleManualDto> MapperlyProductSimpleProjection()
    {
        return _productMapper
            .ProjectToSimpleDto(_productQueryable)
            .ToList();
    }

    #endregion

    #region Complex Query Benchmarks

    [Benchmark]
    public List<UserBasicDto> FacetComplexUserQuery()
    {
        return _userQueryable
            .Where(u => u.IsActive)
            .Where(u => u.Department != null && u.Department.Contains("Engineering"))
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Skip(10)
            .Take(50)
            .SelectFacet<User, UserBasicDto>()
            .ToList();
    }

    [Benchmark]
    public List<UserBasicManualDto> MapsterComplexUserQuery()
    {
        return _userQueryable
            .Where(u => u.IsActive)
            .Where(u => u.Department != null && u.Department.Contains("Engineering"))
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Skip(10)
            .Take(50)
            .ProjectToType<UserBasicManualDto>()
            .ToList();
    }

    [Benchmark]
    public List<UserBasicManualDto> MapperlyComplexUserQuery()
    {
        return _userMapper
            .ProjectToBasicDto(_userQueryable
                .Where(u => u.IsActive)
                .Where(u => u.Department != null && u.Department.Contains("Engineering"))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Skip(10)
                .Take(50))
            .ToList();
    }

    [Benchmark]
    public List<ProductDto> FacetComplexProductQuery()
    {
        return _productQueryable
            .Where(p => p.IsActive)
            .Where(p => p.Price >= 50 && p.Price <= 500)
            .OrderBy(p => p.Name)
            .Take(100)
            .SelectFacet<Product, ProductDto>()
            .ToList();
    }

    [Benchmark]
    public List<ProductManualDto> MapsterComplexProductQuery()
    {
        return _productQueryable
            .Where(p => p.IsActive)
            .Where(p => p.Price >= 50 && p.Price <= 500)
            .OrderBy(p => p.Name)
            .Take(100)
            .ProjectToType<ProductManualDto>()
            .ToList();
    }

    [Benchmark]
    public List<ProductManualDto> MapperlyComplexProductQuery()
    {
        return _productMapper
            .ProjectToDto(_productQueryable
                .Where(p => p.IsActive)
                .Where(p => p.Price >= 50 && p.Price <= 500)
                .OrderBy(p => p.Name)
                .Take(100))
            .ToList();
    }

    #endregion

    #region Expression Compilation Benchmarks

    [Benchmark]
    public int FacetExpressionCompilation()
    {
        // Test expression compilation overhead
        var expression = UserBasicDto.Projection;
        var compiled = expression.Compile();
        return compiled != null ? 1 : 0;
    }

    [Benchmark]
    public int MapsterExpressionCompilation()
    {
        // Test Mapster's expression compilation
        var config = TypeAdapterConfig<User, UserBasicManualDto>.NewConfig();
        return config != null ? 1 : 0;
    }

    #endregion
}