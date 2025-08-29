<div align="center">
  <img
    src="https://raw.githubusercontent.com/Tim-Maes/Facet/master/assets/Facet.png"
    alt="Facet logo"
    width="400">
</div>

<div align="center">
"One part of a subject, situation, object that has many parts."
</div>

---

**Facet** is a C# source generator that lets you define **lightweight projections** (DTOs, API models, etc.) directly from your domain models, without writing boilerplate.

It generates partial classes, records, structs, or record structs with constructors, optional LINQ projections, and even supports custom mappings, all at compile time, with zero runtime cost.

## :gem: What is Facetting?

Facetting is the process of defining **focused views** of a larger model at compile time.

Instead of manually writing separate DTOs, mappers, and projections, **Facet** allows you to declare what you want to keep, and generates everything else.

You can think of it like **carving out a specific facet** of a gem:

- The part you care about
- Leaving the rest behind.

## :grey_question: Why Facetting?

- Reduce duplication across DTOs, projections, and ViewModels
- Maintain strong typing with no runtime cost
- Stay DRY (Don't Repeat Yourself) without sacrificing performance
- Works seamlessly with LINQ providers like Entity Framework

## :clipboard: Documentation

- **[Documentation & Guides](docs/README.md)**
- [What is being generated?](docs/07_WhatIsBeingGenerated.md)

## :star: Key Features

- :white_check_mark: Generate classes, records, structs, or record structs from existing types
- :white_check_mark: Exclude fields/properties you don't want (create a Facetted view of your model)
- :white_check_mark: Include/redact public fields
- :white_check_mark: Auto-generate constructors for fast mapping
- :white_check_mark: LINQ projection expressions
- :white_check_mark: Full mapping support with custom mapping configurations

## :earth_americas: The Facet Ecosystem

Facet is modular and consists of several NuGet packages:

- **Facet**: The core source generator. Generates DTOs, projections, and mapping code.

- **Facet.Extensions**: Provider-agnostic extension methods for mapping and projecting (works with any LINQ provider, no EF Core dependency).

- **Facet.Mapping**: Advanced static mapping configuration support with async capabilities and dependency injection for complex mapping scenarios.

- **Facet.Extensions.EFCore**: Async extension methods for Entity Framework Core (requires EF Core 6+).

## :rocket: Quick start 

### Install the NuGet Package

```
dotnet add package Facet
```

For LINQ helpers:
```
dotnet add package Facet.Extensions
```

For EF Core support:
```
dotnet add package Facet.Extensions.EFCore
```

### Basic Projection
```csharp
[Facet(typeof(User))]
public partial class UserFacet { }

// Auto-generates constructor, properties, and LINQ projection
var user = user.ToFacet<UserFacet>();
var users = users.SelectFacets<UserFacet>();
```

### Property Exclusion & Field Inclusion
```csharp
// Exclude sensitive properties
string[] excludeFields = { "Password", "Email" };

[Facet(typeof(User), exclude: excludeFields)]
public partial class UserWithoutEmail { }

// Include public fields
[Facet(typeof(Entity), IncludeFields = true)]
public partial class EntityDto { }
```

### Different Type Kinds
```csharp
// Generate as record (immutable by default)
[Facet(typeof(Product))]
public partial record ProductDto;

// Generate as struct (value type)
[Facet(typeof(Point))]
public partial struct PointDto;

// Generate as record struct (immutable value type)
[Facet(typeof(Coordinates))]
public partial record struct CoordinatesDto; // Preserves required/init-only
```

### Custom Sync Mapping
```csharp
public class UserMapper : IFacetMapConfiguration<User, UserDto>
{
    public static void Map(User source, UserDto target)
    {
        target.FullName = $"{source.FirstName} {source.LastName}";
        target.Age = CalculateAge(source.DateOfBirth);
    }
}

[Facet(typeof(User), Configuration = typeof(UserMapper))]
public partial class UserDto 
{
    public string FullName { get; set; }
    public int Age { get; set; }
}
```

### Async Mapping for I/O Operations
```csharp
public class UserAsyncMapper : IFacetMapConfigurationAsync<User, UserDto>
{
    public static async Task MapAsync(User source, UserDto target, CancellationToken cancellationToken = default)
    {
        // Async database lookup
        target.ProfilePicture = await GetProfilePictureAsync(source.Id, cancellationToken);
        
        // Async API call
        target.ReputationScore = await CalculateReputationAsync(source.Email, cancellationToken);
    }
}

// Usage
var userDto = await user.ToFacetAsync<User, UserDto, UserAsyncMapper>();
var userDtos = await users.ToFacetsParallelAsync<User, UserDto, UserAsyncMapper>();
```

### Async Mapping with Dependency Injection
```csharp
public class UserAsyncMapperWithDI : IFacetMapConfigurationAsyncInstance<User, UserDto>
{
    private readonly IProfilePictureService _profileService;
    private readonly IReputationService _reputationService;

    public UserAsyncMapperWithDI(IProfilePictureService profileService, IReputationService reputationService)
    {
        _profileService = profileService;
        _reputationService = reputationService;
    }

    public async Task MapAsync(User source, UserDto target, CancellationToken cancellationToken = default)
    {
        // Use injected services
        target.ProfilePicture = await _profileService.GetProfilePictureAsync(source.Id, cancellationToken);
        target.ReputationScore = await _reputationService.CalculateReputationAsync(source.Email, cancellationToken);
    }
}

// Usage with DI
var mapper = new UserAsyncMapperWithDI(profileService, reputationService);
var userDto = await user.ToFacetAsync(mapper);
var userDtos = await users.ToFacetsParallelAsync(mapper);
```

### EF Core Integration

#### Forward Mapping (Entity -> Facet)
```csharp
// Async projection directly in EF Core queries
var userDtos = await dbContext.Users
    .Where(u => u.IsActive)
    .ToFacetsAsync<UserDto>();

// LINQ projection for complex queries
var results = await dbContext.Products
    .Where(p => p.IsAvailable)
    .SelectFacet<ProductDto>()
    .OrderBy(dto => dto.Name)
    .ToListAsync();
```

#### Reverse Mapping (Facet -> Entity)
```csharp
[Facet(typeof(User)]
public partial class UpdateUserDto { }

[HttpPut("{id}")]
public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
{
    var user = await context.Users.FindAsync(id);
    if (user == null) return NotFound();
    
    // Only updates properties that mutated
    user.UpdateFromFacet(dto, context);
    
    await context.SaveChangesAsync();
    return NoContent();
}

// With change tracking for auditing
var result = user.UpdateFromFacetWithChanges(dto, context);
if (result.HasChanges)
{
    logger.LogInformation("User {UserId} updated. Changed: {Properties}", 
        user.Id, string.Join(", ", result.ChangedProperties));
}
```

## Mapping benchmarks

| Method                             | Job        | IterationCount | RunStrategy | UnrollFactor | WarmupCount | Mean         | Error            | StdDev        | Ratio | RatioSD | Allocated | Alloc Ratio |
|----------------------------------- |----------- |--------------- |------------ |------------- |------------ |-------------:|-----------------:|--------------:|------:|--------:|----------:|------------:|
| 'Facet - Single Mapping'           | Job-JLMADU | 2              | Monitoring  | 1            | 1           |  3,300.00 ns | 1,018,590.803 ns |  2,262.742 ns |  1.31 |    1.09 |     536 B |        1.00 |
| 'Mapster - Single Mapping'         | Job-JLMADU | 2              | Monitoring  | 1            | 1           |  4,550.00 ns |   795,774.065 ns |  1,767.767 ns |  1.80 |    1.19 |     528 B |        0.99 |
| 'Mapperly - Single Mapping'        | Job-JLMADU | 2              | Monitoring  | 1            | 1           |  1,600.00 ns |    63,661.925 ns |    141.421 ns |  0.63 |    0.36 |     528 B |        0.99 |
| 'Facet - Collection (10 items)'    | Job-JLMADU | 2              | Monitoring  | 1            | 1           | 12,750.00 ns | 4,806,475.352 ns | 10,677.312 ns |  5.05 |    4.77 |    1968 B |        3.67 |
| 'Mapster - Collection (10 items)'  | Job-JLMADU | 2              | Monitoring  | 1            | 1           |  4,650.00 ns |   541,126.364 ns |  1,202.082 ns |  1.84 |    1.12 |    1816 B |        3.39 |
| 'Mapperly - Collection (10 items)' | Job-JLMADU | 2              | Monitoring  | 1            | 1           | 16,700.00 ns | 6,557,178.295 ns | 14,566.400 ns |  6.62 |    6.41 |    1952 B |        3.64 |
|                                    |            |                |             |              |             |              |                  |               |       |         |           |             |
| 'Facet - Single Mapping'           | Job-UKTOPU | Default        | Default     | 16           | Default     |     15.93 ns |         0.355 ns |      0.449 ns |  1.00 |    0.04 |     136 B |        1.00 |
| 'Mapster - Single Mapping'         | Job-UKTOPU | Default        | Default     | 16           | Default     |     21.90 ns |         0.475 ns |      0.949 ns |  1.38 |    0.07 |     128 B |        0.94 |
| 'Mapperly - Single Mapping'        | Job-UKTOPU | Default        | Default     | 16           | Default     |     15.09 ns |         0.340 ns |      0.848 ns |  0.95 |    0.06 |     128 B |        0.94 |
| 'Facet - Collection (10 items)'    | Job-UKTOPU | Default        | Default     | 16           | Default     |    207.32 ns |         4.182 ns |     10.180 ns | 13.03 |    0.73 |    1568 B |       11.53 |
| 'Mapster - Collection (10 items)'  | Job-UKTOPU | Default        | Default     | 16           | Default     |    192.55 ns |         4.313 ns |     12.512 ns | 12.10 |    0.85 |    1416 B |       10.41 |
| 'Mapperly - Collection (10 items)' | Job-UKTOPU | Default        | Default     | 16           | Default     |    222.50 ns |         4.490 ns |      9.568 ns | 13.98 |    0.71 |    1552 B |       11.41 |
