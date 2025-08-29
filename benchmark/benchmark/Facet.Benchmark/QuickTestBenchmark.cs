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
/// Quick test benchmark with fewer iterations for faster results
/// </summary>
[SimpleJob(RunStrategy.ColdStart, iterationCount: 3, warmupCount: 1)]
[MemoryDiagnoser]
[MarkdownExporter]
public class QuickTestBenchmark
{
    private User _user = null!;
    private UserMapper _userMapper = null!;

    [GlobalSetup]
    public void Setup()
    {
        var testData = TestDataGenerator.CreateTestDataSet(1, 1, 1);
        _user = testData.Users[0];
        _userMapper = new UserMapper();
        
        // Configure Mapster
        TypeAdapterConfig<User, UserBasicManualDto>.NewConfig().Compile();
    }

    [Benchmark(Baseline = true)]
    public UserBasicDto FacetMapping()
    {
        return _user.ToFacet<User, UserBasicDto>();
    }

    [Benchmark]
    public UserBasicManualDto MapsterMapping()
    {
        return _user.Adapt<UserBasicManualDto>();
    }

    [Benchmark]
    public UserBasicManualDto MapperlyMapping()
    {
        return _userMapper.ToBasicDto(_user);
    }
}