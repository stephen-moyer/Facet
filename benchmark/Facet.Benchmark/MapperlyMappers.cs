using Facet.Benchmark.Models;
using Facet.Benchmark.DTOs;
using Riok.Mapperly.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace Facet.Benchmark.Mappers;

/// <summary>
/// Mapperly mappers for benchmarking
/// These use compile-time code generation similar to Facet
/// </summary>

[Mapper]
public partial class UserMapper
{
    public partial UserBasicManualDto ToBasicDto(User user);
    public partial UserDetailedManualDto ToDetailedDto(User user);
    public partial UserSimpleManualDto ToSimpleDto(User user);
    
    // Bulk mapping methods
    public partial IQueryable<UserBasicManualDto> ProjectToBasicDto(IQueryable<User> users);
    public partial IQueryable<UserDetailedManualDto> ProjectToDetailedDto(IQueryable<User> users);
    public partial IQueryable<UserSimpleManualDto> ProjectToSimpleDto(IQueryable<User> users);
    
    // List mapping methods
    public partial List<UserBasicManualDto> ToBasicDtoList(List<User> users);
    public partial List<UserDetailedManualDto> ToDetailedDtoList(List<User> users);
    public partial List<UserSimpleManualDto> ToSimpleDtoList(List<User> users);
}

[Mapper]
public partial class ProductMapper
{
    public partial ProductManualDto ToDto(Product product);
    public partial ProductSimpleManualDto ToSimpleDto(Product product);
    
    // Bulk mapping methods
    public partial IQueryable<ProductManualDto> ProjectToDto(IQueryable<Product> products);
    public partial IQueryable<ProductSimpleManualDto> ProjectToSimpleDto(IQueryable<Product> products);
    
    // List mapping methods
    public partial List<ProductManualDto> ToDtoList(List<Product> products);
    public partial List<ProductSimpleManualDto> ToSimpleDtoList(List<Product> products);
}

[Mapper]
public partial class AddressMapper
{
    public partial AddressManualDto ToDto(Address address);
    
    // Bulk mapping methods
    public partial IQueryable<AddressManualDto> ProjectToDto(IQueryable<Address> addresses);
    
    // List mapping methods
    public partial List<AddressManualDto> ToDtoList(List<Address> addresses);
}

[Mapper]
public partial class OrderMapper
{
    public partial OrderManualDto ToDto(Order order);
    
    // Bulk mapping methods
    public partial IQueryable<OrderManualDto> ProjectToDto(IQueryable<Order> orders);
    
    // List mapping methods
    public partial List<OrderManualDto> ToDtoList(List<Order> orders);
}

[Mapper]
public partial class CategoryMapper
{
    public partial CategoryManualDto ToDto(Category category);
    
    // Bulk mapping methods
    public partial IQueryable<CategoryManualDto> ProjectToDto(IQueryable<Category> categories);
    
    // List mapping methods
    public partial List<CategoryManualDto> ToDtoList(List<Category> categories);
}