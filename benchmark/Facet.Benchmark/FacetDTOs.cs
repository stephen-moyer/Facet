using Facet.Benchmark.Models;

namespace Facet.Benchmark.DTOs;

/// <summary>
/// Basic user DTO using Facet - excludes sensitive information
/// </summary>
[Facet(typeof(User), 
    nameof(User.Salary), 
    nameof(User.Manager), 
    nameof(User.DirectReports), 
    nameof(User.UserRoles))]
public partial class UserBasicDto
{
}

/// <summary>
/// Detailed user DTO using Facet - includes more information but still excludes navigation properties
/// </summary>
[Facet(typeof(User), 
    nameof(User.Manager), 
    nameof(User.DirectReports), 
    nameof(User.UserRoles), 
    nameof(User.Address))]
public partial class UserDetailedDto
{
}

/// <summary>
/// Simple user DTO with only essential fields
/// </summary>
[Facet(typeof(User), 
    nameof(User.PhoneNumber),
    nameof(User.UpdatedAt),
    nameof(User.ManagerId),
    nameof(User.Department),
    nameof(User.JobTitle),
    nameof(User.LastLoginAt),
    nameof(User.LoginCount),
    nameof(User.Salary),
    nameof(User.Manager),
    nameof(User.DirectReports),
    nameof(User.Address),
    nameof(User.UserRoles))]
public partial class UserSimpleDto
{
}

/// <summary>
/// Product DTO using Facet - excludes navigation properties
/// </summary>
[Facet(typeof(Product), 
    nameof(Product.Category), 
    nameof(Product.OrderItems))]
public partial class ProductDto
{
}

/// <summary>
/// Simple product DTO with only basic information
/// </summary>
[Facet(typeof(Product), 
    nameof(Product.Description),
    nameof(Product.StockQuantity),
    nameof(Product.SKU),
    nameof(Product.UpdatedAt),
    nameof(Product.CategoryId),
    nameof(Product.Category),
    nameof(Product.OrderItems))]
public partial class ProductSimpleDto
{
}

/// <summary>
/// Address DTO using Facet - excludes navigation properties
/// </summary>
[Facet(typeof(Address), nameof(Address.User))]
public partial class AddressDto
{
}

/// <summary>
/// Order DTO using Facet - excludes navigation properties
/// </summary>
[Facet(typeof(Order), 
    nameof(Order.User), 
    nameof(Order.OrderItems))]
public partial class OrderDto
{
}

/// <summary>
/// Category DTO using Facet - excludes navigation properties
/// </summary>
[Facet(typeof(Category), nameof(Category.Products))]
public partial class CategoryDto
{
}