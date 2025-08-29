using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Facet.Benchmark.Models;

/// <summary>
/// Represents a user entity with comprehensive properties for benchmarking
/// </summary>
public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public bool IsActive { get; set; }
    
    public decimal Salary { get; set; }
    
    public int? ManagerId { get; set; }
    
    public string? Department { get; set; }
    
    public string? JobTitle { get; set; }
    
    public DateTime? LastLoginAt { get; set; }
    
    public int LoginCount { get; set; }
    
    // Navigation properties for more complex scenarios
    public virtual User? Manager { get; set; }
    
    public virtual ICollection<User> DirectReports { get; set; } = new List<User>();
    
    public virtual Address? Address { get; set; }
    
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

/// <summary>
/// Represents an address entity
/// </summary>
public class Address
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Street { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string PostalCode { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;
    
    public string? State { get; set; }
    
    public virtual User User { get; set; } = null!;
}

/// <summary>
/// Represents a role entity
/// </summary>
public class Role
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

/// <summary>
/// Represents the many-to-many relationship between users and roles
/// </summary>
public class UserRole
{
    public int UserId { get; set; }
    
    public int RoleId { get; set; }
    
    public DateTime AssignedAt { get; set; }
    
    public virtual User User { get; set; } = null!;
    
    public virtual Role Role { get; set; } = null!;
}

/// <summary>
/// Represents a product entity for additional benchmarking scenarios
/// </summary>
public class Product
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    public int StockQuantity { get; set; }
    
    [StringLength(50)]
    public string? SKU { get; set; }
    
    public bool IsActive { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
    
    public int CategoryId { get; set; }
    
    public virtual Category Category { get; set; } = null!;
    
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

/// <summary>
/// Represents a category entity
/// </summary>
public class Category
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

/// <summary>
/// Represents an order entity
/// </summary>
public class Order
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public decimal TotalAmount { get; set; }
    
    public string Status { get; set; } = "Pending";
    
    public string? Notes { get; set; }
    
    public virtual User User { get; set; } = null!;
    
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

/// <summary>
/// Represents an order item entity
/// </summary>
public class OrderItem
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    public int ProductId { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
    
    public virtual Order Order { get; set; } = null!;
    
    public virtual Product Product { get; set; } = null!;
}