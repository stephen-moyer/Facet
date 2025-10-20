namespace Facet.Tests.TestModels;

// Source entities for complex type testing
public class AddressEntity
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class CompanyEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public AddressEntity HeadquartersAddress { get; set; } = new();
}

public class StaffMember
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public CompanyEntity Company { get; set; } = new();
    public AddressEntity HomeAddress { get; set; } = new();
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
}

public class DepartmentEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public CompanyEntity Company { get; set; } = new();
    public StaffMember Manager { get; set; } = new();
    public int EmployeeCount { get; set; }
}

// Facet DTOs
[Facet(typeof(AddressEntity))]
public partial record AddressFacet;

[Facet(
    typeof(CompanyEntity),
    NestedFacets = [typeof(AddressFacet)])]
public partial record CompanyFacet;

[Facet(
    typeof(StaffMember),
    exclude: ["PasswordHash", "Salary"],
    NestedFacets = [typeof(CompanyFacet), typeof(AddressFacet)])]
public partial record StaffMemberFacet;

[Facet(typeof(DepartmentEntity), NestedFacets = [typeof(CompanyFacet), typeof(StaffMemberFacet)])]
public partial record DepartmentFacet;

// Collection nested facets test models
public class OrderItemEntity
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class OrderEntity
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public List<OrderItemEntity> Items { get; set; } = new();
    public AddressEntity ShippingAddress { get; set; } = new();
}

public class TeamEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public StaffMember[] Members { get; set; } = Array.Empty<StaffMember>();
}

public class ProjectEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<TeamEntity> Teams { get; set; } = new List<TeamEntity>();
}

// Collection nested facet DTOs
[Facet(typeof(OrderItemEntity))]
public partial record OrderItemFacet;

[Facet(typeof(OrderEntity), NestedFacets = [typeof(OrderItemFacet), typeof(AddressFacet)])]
public partial record OrderFacet;

[Facet(typeof(TeamEntity), NestedFacets = [typeof(StaffMemberFacet)])]
public partial record TeamFacet;

[Facet(typeof(ProjectEntity), NestedFacets = [typeof(TeamFacet)])]
public partial record ProjectFacet;
