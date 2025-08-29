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
/// Benchmarks collection mapping performance with different collection sizes
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
public class CollectionMappingBenchmark
{
    private List<User> _users10 = null!;
    private List<User> _users100 = null!;
    private List<User> _users1000 = null!;
    
    private List<Product> _products10 = null!;
    private List<Product> _products100 = null!;
    private List<Product> _products1000 = null!;
    
    private UserMapper _userMapper = null!;
    private ProductMapper _productMapper = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Generate test data
        _users10 = TestDataGenerator.GenerateUsers(10);
        _users100 = TestDataGenerator.GenerateUsers(100);
        _users1000 = TestDataGenerator.GenerateUsers(1000);
        
        _products10 = TestDataGenerator.GenerateProducts(10);
        _products100 = TestDataGenerator.GenerateProducts(100);
        _products1000 = TestDataGenerator.GenerateProducts(1000);
        
        _userMapper = new UserMapper();
        _productMapper = new ProductMapper();
        
        // Configure Mapster
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

    #region User Collection Benchmarks - 10 items

    [Benchmark]
    [Arguments(10)]
    public List<UserBasicDto> FacetUsers10Basic()
    {
        return _users10.Select(u => u.ToFacet<UserBasicDto>()).ToList();
    }

    [Benchmark]
    [Arguments(10)]
    public List<UserBasicManualDto> MapsterUsers10Basic()
    {
        return _users10.Adapt<List<UserBasicManualDto>>();
    }

    [Benchmark]
    [Arguments(10)]
    public List<UserBasicManualDto> MapperlyUsers10Basic()
    {
        return _userMapper.ToBasicDtoList(_users10);
    }

    [Benchmark]
    [Arguments(10)]
    public List<UserSimpleDto> FacetUsers10Simple()
    {
        return _users10.Select(u => u.ToFacet<UserSimpleDto>()).ToList();
    }

    [Benchmark]
    [Arguments(10)]
    public List<UserSimpleManualDto> MapsterUsers10Simple()
    {
        return _users10.Adapt<List<UserSimpleManualDto>>();
    }

    [Benchmark]
    [Arguments(10)]
    public List<UserSimpleManualDto> MapperlyUsers10Simple()
    {
        return _userMapper.ToSimpleDtoList(_users10);
    }

    #endregion

    #region User Collection Benchmarks - 100 items

    [Benchmark(Baseline = true)]
    [Arguments(100)]
    public List<UserBasicDto> FacetUsers100Basic()
    {
        return _users100.Select(u => u.ToFacet<UserBasicDto>()).ToList();
    }

    [Benchmark]
    [Arguments(100)]
    public List<UserBasicManualDto> MapsterUsers100Basic()
    {
        return _users100.Adapt<List<UserBasicManualDto>>();
    }

    [Benchmark]
    [Arguments(100)]
    public List<UserBasicManualDto> MapperlyUsers100Basic()
    {
        return _userMapper.ToBasicDtoList(_users100);
    }

    [Benchmark]
    [Arguments(100)]
    public List<UserSimpleDto> FacetUsers100Simple()
    {
        return _users100.Select(u => u.ToFacet<UserSimpleDto>()).ToList();
    }

    [Benchmark]
    [Arguments(100)]
    public List<UserSimpleManualDto> MapsterUsers100Simple()
    {
        return _users100.Adapt<List<UserSimpleManualDto>>();
    }

    [Benchmark]
    [Arguments(100)]
    public List<UserSimpleManualDto> MapperlyUsers100Simple()
    {
        return _userMapper.ToSimpleDtoList(_users100);
    }

    #endregion

    #region User Collection Benchmarks - 1000 items

    [Benchmark]
    [Arguments(1000)]
    public List<UserBasicDto> FacetUsers1000Basic()
    {
        return _users1000.Select(u => u.ToFacet<UserBasicDto>()).ToList();
    }

    [Benchmark]
    [Arguments(1000)]
    public List<UserBasicManualDto> MapsterUsers1000Basic()
    {
        return _users1000.Adapt<List<UserBasicManualDto>>();
    }

    [Benchmark]
    [Arguments(1000)]
    public List<UserBasicManualDto> MapperlyUsers1000Basic()
    {
        return _userMapper.ToBasicDtoList(_users1000);
    }

    [Benchmark]
    [Arguments(1000)]
    public List<UserSimpleDto> FacetUsers1000Simple()
    {
        return _users1000.Select(u => u.ToFacet<UserSimpleDto>()).ToList();
    }

    [Benchmark]
    [Arguments(1000)]
    public List<UserSimpleManualDto> MapsterUsers1000Simple()
    {
        return _users1000.Adapt<List<UserSimpleManualDto>>();
    }

    [Benchmark]
    [Arguments(1000)]
    public List<UserSimpleManualDto> MapperlyUsers1000Simple()
    {
        return _userMapper.ToSimpleDtoList(_users1000);
    }

    #endregion

    #region Product Collection Benchmarks - 100 items

    [Benchmark]
    [Arguments(100)]
    public List<ProductDto> FacetProducts100()
    {
        return _products100.Select(p => p.ToFacet<ProductDto>()).ToList();
    }

    [Benchmark]
    [Arguments(100)]
    public List<ProductManualDto> MapsterProducts100()
    {
        return _products100.Adapt<List<ProductManualDto>>();
    }

    [Benchmark]
    [Arguments(100)]
    public List<ProductManualDto> MapperlyProducts100()
    {
        return _productMapper.ToDtoList(_products100);
    }

    [Benchmark]
    [Arguments(100)]
    public List<ProductSimpleDto> FacetProducts100Simple()
    {
        return _products100.Select(p => p.ToFacet<ProductSimpleDto>()).ToList();
    }

    [Benchmark]
    [Arguments(100)]
    public List<ProductSimpleManualDto> MapsterProducts100Simple()
    {
        return _products100.Adapt<List<ProductSimpleManualDto>>();
    }

    [Benchmark]
    [Arguments(100)]
    public List<ProductSimpleManualDto> MapperlyProducts100Simple()
    {
        return _productMapper.ToSimpleDtoList(_products100);
    }

    #endregion

    #region Product Collection Benchmarks - 1000 items

    [Benchmark]
    [Arguments(1000)]
    public List<ProductDto> FacetProducts1000()
    {
        return _products1000.Select(p => p.ToFacet<ProductDto>()).ToList();
    }

    [Benchmark]
    [Arguments(1000)]
    public List<ProductManualDto> MapsterProducts1000()
    {
        return _products1000.Adapt<List<ProductManualDto>>();
    }

    [Benchmark]
    [Arguments(1000)]
    public List<ProductManualDto> MapperlyProducts1000()
    {
        return _productMapper.ToDtoList(_products1000);
    }

    [Benchmark]
    [Arguments(1000)]
    public List<ProductSimpleDto> FacetProducts1000Simple()
    {
        return _products1000.Select(p => p.ToFacet<ProductSimpleDto>()).ToList();
    }

    [Benchmark]
    [Arguments(1000)]
    public List<ProductSimpleManualDto> MapsterProducts1000Simple()
    {
        return _products1000.Adapt<List<ProductSimpleManualDto>>();
    }

    [Benchmark]
    [Arguments(1000)]
    public List<ProductSimpleManualDto> MapperlyProducts1000Simple()
    {
        return _productMapper.ToSimpleDtoList(_products1000);
    }

    #endregion
}