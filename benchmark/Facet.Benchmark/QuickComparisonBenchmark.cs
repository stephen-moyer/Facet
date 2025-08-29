using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Facet.Benchmark.Models;
using Facet.Benchmark.DTOs;
using Facet.Benchmark.Mappers;
using Facet.Extensions;
using Mapster;
using System.Collections.Generic;
using System.Linq;

namespace Facet.Benchmark.Benchmarks;

/// <summary>
/// Quick comparison benchmark with minimal test scenarios for fast evaluation
/// Perfect for quick testing and comparison between Facet, Mapster, and Mapperly
/// </summary>
[SimpleJob(RunStrategy.Monitoring, warmupCount: 1, iterationCount: 5)]
[MemoryDiagnoser]
[MarkdownExporter]
[JsonExporter]
public class QuickComparisonBenchmark
{
    private User _user = null!;
    private Product _product = null!;
    private List<User> _users = null!;
    private List<Product> _products = null!;
    
    private UserMapper _userMapper = null!;
    private ProductMapper _productMapper = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Generate minimal test data for quick testing
        var testData = TestDataGenerator.CreateTestDataSet(1, 1, 1);
        _user = testData.Users[0];
        _product = testData.Products[0];
        
        // Small collections for quick testing
        _users = TestDataGenerator.GenerateUsers(25);
        _products = TestDataGenerator.GenerateProducts(25);
        
        _userMapper = new UserMapper();
        _productMapper = new ProductMapper();
        
        // Configure Mapster
        ConfigureMapster();
    }

    private static void ConfigureMapster()
    {
        TypeAdapterConfig<User, UserBasicManualDto>.NewConfig().Compile();
        TypeAdapterConfig<User, UserSimpleManualDto>.NewConfig().Compile();
        TypeAdapterConfig<Product, ProductManualDto>.NewConfig().Compile();
        TypeAdapterConfig<Product, ProductSimpleManualDto>.NewConfig().Compile();
    }

    #region Single Entity Quick Comparison

    [Benchmark(Baseline = true, Description = "Facet single user mapping")]
    public UserBasicDto FacetUserBasic()
    {
        return _user.ToFacet<UserBasicDto>();
    }

    [Benchmark(Description = "Mapster single user mapping")]
    public UserBasicManualDto MapsterUserBasic()
    {
        return _user.Adapt<UserBasicManualDto>();
    }

    [Benchmark(Description = "Mapperly single user mapping")]
    public UserBasicManualDto MapperlyUserBasic()
    {
        return _userMapper.ToBasicDto(_user);
    }

    [Benchmark(Description = "Facet single product mapping")]
    public ProductDto FacetProduct()
    {
        return _product.ToFacet<ProductDto>();
    }

    [Benchmark(Description = "Mapster single product mapping")]
    public ProductManualDto MapsterProduct()
    {
        return _product.Adapt<ProductManualDto>();
    }

    [Benchmark(Description = "Mapperly single product mapping")]
    public ProductManualDto MapperlyProduct()
    {
        return _productMapper.ToDto(_product);
    }

    #endregion

    #region Small Collection Quick Comparison

    [Benchmark(Description = "Facet 25 users collection mapping")]
    public List<UserBasicDto> FacetUsers25()
    {
        return _users.Select(u => u.ToFacet<UserBasicDto>()).ToList();
    }

    [Benchmark(Description = "Mapster 25 users collection mapping")]
    public List<UserBasicManualDto> MapsterUsers25()
    {
        return _users.Adapt<List<UserBasicManualDto>>();
    }

    [Benchmark(Description = "Mapperly 25 users collection mapping")]
    public List<UserBasicManualDto> MapperlyUsers25()
    {
        return _users.Select(_userMapper.ToBasicDto).ToList();
    }

    [Benchmark(Description = "Facet 25 products collection mapping")]
    public List<ProductDto> FacetProducts25()
    {
        return _products.Select(p => p.ToFacet<ProductDto>()).ToList();
    }

    [Benchmark(Description = "Mapster 25 products collection mapping")]
    public List<ProductManualDto> MapsterProducts25()
    {
        return _products.Adapt<List<ProductManualDto>>();
    }

    [Benchmark(Description = "Mapperly 25 products collection mapping")]
    public List<ProductManualDto> MapperlyProducts25()
    {
        return _products.Select(_productMapper.ToDto).ToList();
    }

    #endregion

    #region LINQ Projection Quick Comparison

    [Benchmark(Description = "Facet LINQ projection")]
    public List<UserBasicDto> FacetLinqProjection()
    {
        return _users.AsQueryable()
            .Where(u => u.IsActive)
            .Select(u => u.ToFacet<UserBasicDto>())
            .ToList();
    }

    [Benchmark(Description = "Mapster LINQ projection")]
    public List<UserBasicManualDto> MapsterLinqProjection()
    {
        return _users.AsQueryable()
            .Where(u => u.IsActive)
            .ProjectToType<UserBasicManualDto>()
            .ToList();
    }

    [Benchmark(Description = "Mapperly LINQ projection")]
    public List<UserBasicManualDto> MapperlyLinqProjection()
    {
        return _userMapper
            .ProjectToBasicDto(_users.AsQueryable().Where(u => u.IsActive))
            .ToList();
    }

    #endregion
}