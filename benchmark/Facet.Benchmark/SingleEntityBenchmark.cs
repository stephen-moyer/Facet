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
/// Benchmarks single entity mapping performance
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
public class SingleEntityMappingBenchmark
{
    private User _user = null!;
    private Product _product = null!;
    private Address _address = null!;
    private Order _order = null!;
    private Category _category = null!;
    
    private UserMapper _userMapper = null!;
    private ProductMapper _productMapper = null!;
    private AddressMapper _addressMapper = null!;
    private OrderMapper _orderMapper = null!;
    private CategoryMapper _categoryMapper = null!;

    [GlobalSetup]
    public void Setup()
    {
        var testData = TestDataGenerator.CreateTestDataSet(1, 1, 1);
        _user = testData.Users[0];
        _product = testData.Products[0];
        _address = testData.Addresses[0];
        _order = testData.Orders[0];
        _category = testData.Categories[0];
        
        _userMapper = new UserMapper();
        _productMapper = new ProductMapper();
        _addressMapper = new AddressMapper();
        _orderMapper = new OrderMapper();
        _categoryMapper = new CategoryMapper();
        
        // Configure Mapster
        ConfigureMapster();
    }

    private static void ConfigureMapster()
    {
        // Configure Mapster mappings - just use default mappings without any special configuration
        TypeAdapterConfig<User, UserBasicManualDto>.NewConfig()
            .Compile();
            
        TypeAdapterConfig<User, UserDetailedManualDto>.NewConfig()
            .Compile();
            
        TypeAdapterConfig<User, UserSimpleManualDto>.NewConfig()
            .Compile();
            
        TypeAdapterConfig<Product, ProductManualDto>.NewConfig()
            .Compile();
            
        TypeAdapterConfig<Product, ProductSimpleManualDto>.NewConfig()
            .Compile();
            
        TypeAdapterConfig<Address, AddressManualDto>.NewConfig()
            .Compile();
            
        TypeAdapterConfig<Order, OrderManualDto>.NewConfig()
            .Compile();
            
        TypeAdapterConfig<Category, CategoryManualDto>.NewConfig()
            .Compile();
    }

    #region User Mapping Benchmarks

    [Benchmark(Baseline = true)]
    public UserBasicDto FacetUserBasic()
    {
        return _user.ToFacet<UserBasicDto>();
    }

    [Benchmark]
    public UserBasicManualDto MapsterUserBasic()
    {
        return _user.Adapt<UserBasicManualDto>();
    }

    [Benchmark]
    public UserBasicManualDto MapperlyUserBasic()
    {
        return _userMapper.ToBasicDto(_user);
    }

    [Benchmark]
    public UserDetailedDto FacetUserDetailed()
    {
        return _user.ToFacet<UserDetailedDto>();
    }

    [Benchmark]
    public UserDetailedManualDto MapsterUserDetailed()
    {
        return _user.Adapt<UserDetailedManualDto>();
    }

    [Benchmark]
    public UserDetailedManualDto MapperlyUserDetailed()
    {
        return _userMapper.ToDetailedDto(_user);
    }

    [Benchmark]
    public UserSimpleDto FacetUserSimple()
    {
        return _user.ToFacet<UserSimpleDto>();
    }

    [Benchmark]
    public UserSimpleManualDto MapsterUserSimple()
    {
        return _user.Adapt<UserSimpleManualDto>();
    }

    [Benchmark]
    public UserSimpleManualDto MapperlyUserSimple()
    {
        return _userMapper.ToSimpleDto(_user);
    }

    #endregion

    #region Product Mapping Benchmarks

    [Benchmark]
    public ProductDto FacetProduct()
    {
        return _product.ToFacet<ProductDto>();
    }

    [Benchmark]
    public ProductManualDto MapsterProduct()
    {
        return _product.Adapt<ProductManualDto>();
    }

    [Benchmark]
    public ProductManualDto MapperlyProduct()
    {
        return _productMapper.ToDto(_product);
    }

    [Benchmark]
    public ProductSimpleDto FacetProductSimple()
    {
        return _product.ToFacet<ProductSimpleDto>();
    }

    [Benchmark]
    public ProductSimpleManualDto MapsterProductSimple()
    {
        return _product.Adapt<ProductSimpleManualDto>();
    }

    [Benchmark]
    public ProductSimpleManualDto MapperlyProductSimple()
    {
        return _productMapper.ToSimpleDto(_product);
    }

    #endregion

    #region Other Entity Mapping Benchmarks

    [Benchmark]
    public AddressDto FacetAddress()
    {
        return _address.ToFacet<AddressDto>();
    }

    [Benchmark]
    public AddressManualDto MapsterAddress()
    {
        return _address.Adapt<AddressManualDto>();
    }

    [Benchmark]
    public AddressManualDto MapperlyAddress()
    {
        return _addressMapper.ToDto(_address);
    }

    [Benchmark]
    public OrderDto FacetOrder()
    {
        return _order.ToFacet<OrderDto>();
    }

    [Benchmark]
    public OrderManualDto MapsterOrder()
    {
        return _order.Adapt<OrderManualDto>();
    }

    [Benchmark]
    public OrderManualDto MapperlyOrder()
    {
        return _orderMapper.ToDto(_order);
    }

    [Benchmark]
    public CategoryDto FacetCategory()
    {
        return _category.ToFacet<CategoryDto>();
    }

    [Benchmark]
    public CategoryManualDto MapsterCategory()
    {
        return _category.Adapt<CategoryManualDto>();
    }

    [Benchmark]
    public CategoryManualDto MapperlyCategory()
    {
        return _categoryMapper.ToDto(_category);
    }

    #endregion
}