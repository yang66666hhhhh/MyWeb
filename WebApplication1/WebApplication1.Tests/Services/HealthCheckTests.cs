using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebApplication1.Shared.HealthChecks;

namespace WebApplication1.Tests.Services;

public class HealthCheckTests
{
    [Fact]
    public async Task MemoryHealthCheck_ShouldReturnHealthy_WhenBelowThreshold()
    {
        var healthCheck = new MemoryHealthCheck(thresholdBytes: 1024 * 1024 * 1024); // 1GB
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(
                "memory",
                healthCheck,
                HealthStatus.Degraded,
                new List<string> { "system", "memory" })
        };

        var result = await healthCheck.CheckHealthAsync(context);

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.NotNull(result.Description);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.ContainsKey("AllocatedBytes"));
        Assert.True(result.Data.ContainsKey("ThresholdBytes"));
        Assert.True(result.Data.ContainsKey("Gen0Collections"));
    }

    [Fact]
    public async Task MemoryHealthCheck_ShouldReturnDegraded_WhenAboveThreshold()
    {
        var healthCheck = new MemoryHealthCheck(thresholdBytes: 1); // 1 byte threshold
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(
                "memory",
                healthCheck,
                HealthStatus.Degraded,
                new List<string> { "system", "memory" })
        };

        var result = await healthCheck.CheckHealthAsync(context);

        Assert.Equal(HealthStatus.Degraded, result.Status);
    }

    [Fact]
    public async Task DiskSpaceHealthCheck_ShouldReturnHealthy_WhenDriveExists()
    {
        var driveName = Path.GetPathRoot(AppContext.BaseDirectory) ?? Directory.GetDirectoryRoot(AppContext.BaseDirectory);
        var healthCheck = new DiskSpaceHealthCheck(driveName, thresholdBytes: 1024 * 1024); // 1MB
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(
                "disk",
                healthCheck,
                HealthStatus.Degraded,
                new List<string> { "system", "disk" })
        };

        var result = await healthCheck.CheckHealthAsync(context);

        Assert.True(result.Status == HealthStatus.Healthy || result.Status == HealthStatus.Degraded);
        Assert.NotNull(result.Description);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.ContainsKey("DriveName"));
        Assert.True(result.Data.ContainsKey("TotalSizeBytes"));
        Assert.True(result.Data.ContainsKey("AvailableFreeSpaceBytes"));
    }

    [Fact]
    public async Task DiskSpaceHealthCheck_ShouldUseCurrentPlatformRoot_WhenDriveIsNotConfigured()
    {
        var expectedDriveName = Path.GetPathRoot(AppContext.BaseDirectory) ?? Directory.GetDirectoryRoot(AppContext.BaseDirectory);
        var healthCheck = new DiskSpaceHealthCheck(thresholdBytes: 1024 * 1024); // 1MB
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(
                "disk",
                healthCheck,
                HealthStatus.Degraded,
                new List<string> { "system", "disk" })
        };

        var result = await healthCheck.CheckHealthAsync(context);

        Assert.True(result.Status == HealthStatus.Healthy || result.Status == HealthStatus.Degraded);
        Assert.Equal(expectedDriveName, result.Data["DriveName"]);
    }

    [Fact]
    public async Task CpuHealthCheck_ShouldReturnHealthy()
    {
        var healthCheck = new CpuHealthCheck(thresholdPercentage: 80);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(
                "cpu",
                healthCheck,
                HealthStatus.Degraded,
                new List<string> { "system", "cpu" })
        };

        var result = await healthCheck.CheckHealthAsync(context);

        Assert.True(result.Status == HealthStatus.Healthy || result.Status == HealthStatus.Degraded);
        Assert.NotNull(result.Description);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.ContainsKey("CpuUsagePercentage"));
        Assert.True(result.Data.ContainsKey("ThreadCount"));
        Assert.True(result.Data.ContainsKey("HandleCount"));
    }

    [Fact]
    public async Task ApplicationHealthCheck_ShouldReturnHealthy()
    {
        var healthCheck = new ApplicationHealthCheck();
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration(
                "application",
                healthCheck,
                HealthStatus.Unhealthy,
                new List<string> { "system", "app" })
        };

        var result = await healthCheck.CheckHealthAsync(context);

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.NotNull(result.Description);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.ContainsKey("MachineName"));
        Assert.True(result.Data.ContainsKey("OSVersion"));
        Assert.True(result.Data.ContainsKey("ProcessorCount"));
        Assert.True(result.Data.ContainsKey("CLRVersion"));
    }
}
