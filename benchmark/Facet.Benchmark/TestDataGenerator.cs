using Facet.Benchmark.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Facet.Benchmark;

/// <summary>
/// Generates test data for benchmarking
/// </summary>
public static class TestDataGenerator
{
    private static readonly Random _random = new(42); // Fixed seed for reproducible results
    
    private static readonly string[] _firstNames = 
    {
        "John", "Jane", "Michael", "Sarah", "David", "Emily", "Christopher", "Jessica", 
        "Daniel", "Ashley", "Matthew", "Amanda", "Andrew", "Stephanie", "Joshua", "Melissa",
        "Robert", "Nicole", "Nicholas", "Elizabeth", "Ryan", "Heather", "Jacob", "Amy",
        "Tyler", "Michelle", "Aaron", "Kimberly", "Jose", "Angela", "Adam", "Brenda"
    };
    
    private static readonly string[] _lastNames = 
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
        "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas",
        "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White",
        "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson", "Walker", "Young"
    };
    
    private static readonly string[] _departments = 
    {
        "Engineering", "Marketing", "Sales", "Human Resources", "Finance", "Operations",
        "Customer Service", "Product Management", "Quality Assurance", "Research and Development"
    };
    
    private static readonly string[] _jobTitles = 
    {
        "Software Engineer", "Senior Developer", "Project Manager", "Product Manager", "Designer",
        "Data Analyst", "Marketing Specialist", "Sales Representative", "HR Coordinator", "Accountant",
        "Operations Manager", "Customer Support", "QA Engineer", "Research Scientist", "Technical Lead"
    };
    
    private static readonly string[] _cities = 
    {
        "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio",
        "San Diego", "Dallas", "San Jose", "Austin", "Jacksonville", "Fort Worth", "Columbus",
        "Charlotte", "Seattle", "Denver", "El Paso", "Detroit", "Boston", "Memphis", "Portland"
    };
    
    private static readonly string[] _countries = 
    {
        "United States", "Canada", "United Kingdom", "Germany", "France", "Australia", "Japan", "Brazil"
    };
    
    private static readonly string[] _productNames = 
    {
        "Wireless Headphones", "Smart Watch", "Laptop Computer", "Coffee Maker", "Smartphone",
        "Tablet", "Gaming Console", "Bluetooth Speaker", "Digital Camera", "Fitness Tracker",
        "Air Purifier", "Electric Toothbrush", "Vacuum Cleaner", "Microwave Oven", "Blender"
    };
    
    private static readonly string[] _categoryNames = 
    {
        "Electronics", "Home Appliances", "Fitness", "Gaming", "Audio", "Computing", "Mobile", "Health"
    };

    public static List<User> GenerateUsers(int count)
    {
        var users = new List<User>(count);
        
        for (int i = 1; i <= count; i++)
        {
            var firstName = _firstNames[_random.Next(_firstNames.Length)];
            var lastName = _lastNames[_random.Next(_lastNames.Length)];
            
            users.Add(new User
            {
                Id = i,
                FirstName = firstName,
                LastName = lastName,
                Email = $"{firstName.ToLower()}.{lastName.ToLower()}@company.com",
                PhoneNumber = GeneratePhoneNumber(),
                CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 1000)),
                UpdatedAt = _random.NextDouble() > 0.3 ? DateTime.UtcNow.AddDays(-_random.Next(1, 30)) : null,
                IsActive = _random.NextDouble() > 0.1, // 90% active
                Salary = _random.Next(40000, 200000),
                ManagerId = i > 10 ? _random.Next(1, Math.Min(10, i)) : null, // Some users have managers
                Department = _departments[_random.Next(_departments.Length)],
                JobTitle = _jobTitles[_random.Next(_jobTitles.Length)],
                LastLoginAt = _random.NextDouble() > 0.2 ? DateTime.UtcNow.AddDays(-_random.Next(1, 30)) : null,
                LoginCount = _random.Next(0, 1000)
            });
        }
        
        return users;
    }
    
    public static List<Product> GenerateProducts(int count)
    {
        var products = new List<Product>(count);
        
        for (int i = 1; i <= count; i++)
        {
            products.Add(new Product
            {
                Id = i,
                Name = $"{_productNames[_random.Next(_productNames.Length)]} {i}",
                Description = $"High-quality {_productNames[_random.Next(_productNames.Length)].ToLower()} with advanced features.",
                Price = (decimal)(_random.NextDouble() * 2000 + 10), // $10 - $2010
                StockQuantity = _random.Next(0, 1000),
                SKU = $"SKU{i:D6}",
                IsActive = _random.NextDouble() > 0.05, // 95% active
                CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                UpdatedAt = _random.NextDouble() > 0.4 ? DateTime.UtcNow.AddDays(-_random.Next(1, 30)) : null,
                CategoryId = _random.Next(1, 9) // Assuming 8 categories
            });
        }
        
        return products;
    }
    
    public static List<Address> GenerateAddresses(int count)
    {
        var addresses = new List<Address>(count);
        
        for (int i = 1; i <= count; i++)
        {
            addresses.Add(new Address
            {
                Id = i,
                UserId = i, // One address per user
                Street = $"{_random.Next(1, 9999)} {_lastNames[_random.Next(_lastNames.Length)]} Street",
                City = _cities[_random.Next(_cities.Length)],
                PostalCode = $"{_random.Next(10000, 99999)}",
                Country = _countries[_random.Next(_countries.Length)],
                State = _random.NextDouble() > 0.5 ? $"State{_random.Next(1, 51)}" : null
            });
        }
        
        return addresses;
    }
    
    public static List<Category> GenerateCategories()
    {
        var categories = new List<Category>();
        
        for (int i = 1; i <= _categoryNames.Length; i++)
        {
            categories.Add(new Category
            {
                Id = i,
                Name = _categoryNames[i - 1],
                Description = $"Products in the {_categoryNames[i - 1].ToLower()} category"
            });
        }
        
        return categories;
    }
    
    public static List<Order> GenerateOrders(int count, int maxUserId)
    {
        var orders = new List<Order>(count);
        var statuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
        
        for (int i = 1; i <= count; i++)
        {
            orders.Add(new Order
            {
                Id = i,
                UserId = _random.Next(1, maxUserId + 1),
                OrderDate = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                TotalAmount = (decimal)(_random.NextDouble() * 1000 + 10), // $10 - $1010
                Status = statuses[_random.Next(statuses.Length)],
                Notes = _random.NextDouble() > 0.7 ? "Special delivery instructions" : null
            });
        }
        
        return orders;
    }
    
    private static string GeneratePhoneNumber()
    {
        return $"+1-{_random.Next(200, 999)}-{_random.Next(200, 999)}-{_random.Next(1000, 9999)}";
    }
    
    /// <summary>
    /// Creates a comprehensive test dataset
    /// </summary>
    public static TestDataSet CreateTestDataSet(int userCount = 1000, int productCount = 500, int orderCount = 2000)
    {
        return new TestDataSet
        {
            Users = GenerateUsers(userCount),
            Products = GenerateProducts(productCount),
            Addresses = GenerateAddresses(userCount), // One address per user
            Categories = GenerateCategories(),
            Orders = GenerateOrders(orderCount, userCount)
        };
    }
}

/// <summary>
/// Container for all test data
/// </summary>
public class TestDataSet
{
    public List<User> Users { get; set; } = new();
    public List<Product> Products { get; set; } = new();
    public List<Address> Addresses { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
}