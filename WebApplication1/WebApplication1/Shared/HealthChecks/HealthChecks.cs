using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace WebApplication1.Shared.HealthChecks;

public class MemoryHealthCheck : IHealthCheck
{
    private readonly long _thresholdBytes;

    public MemoryHealthCheck(long thresholdBytes = 1024 * 1024 * 1024) // 1GB default
    {
        _thresholdBytes = thresholdBytes;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var allocated = GC.GetTotalMemory(forceFullCollection: false);
        var data = new Dictionary<string, object>
        {
            { "AllocatedBytes", allocated },
            { "ThresholdBytes", _thresholdBytes },
            { "Gen0Collections", GC.CollectionCount(0) },
            { "Gen1Collections", GC.CollectionCount(1) },
            { "Gen2Collections", GC.CollectionCount(2) }
        };

        var status = allocated < _thresholdBytes
            ? HealthStatus.Healthy
            : HealthStatus.Degraded;

        return Task.FromResult(new HealthCheckResult(
            status,
            description: $"Memory usage: {allocated / 1024 / 1024}MB / {_thresholdBytes / 1024 / 1024}MB",
            data: data));
    }
}

public class DiskSpaceHealthCheck : IHealthCheck
{
    private readonly long _thresholdBytes;
    private readonly string _driveName;

    public DiskSpaceHealthCheck(string? driveName = null, long thresholdBytes = 1024 * 1024 * 1024) // 1GB default
    {
        _driveName = string.IsNullOrWhiteSpace(driveName) ? GetDefaultDriveName() : driveName;
        _thresholdBytes = thresholdBytes;
    }

    private static string GetDefaultDriveName()
    {
        return Path.GetPathRoot(AppContext.BaseDirectory)
            ?? Directory.GetDirectoryRoot(AppContext.BaseDirectory);
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var drive = new DriveInfo(_driveName);
            var availableFreeSpace = drive.AvailableFreeSpace;
            var totalSize = drive.TotalSize;
            var usedSpace = totalSize - availableFreeSpace;
            var usagePercentage = (double)usedSpace / totalSize * 100;

            var data = new Dictionary<string, object>
            {
                { "DriveName", _driveName },
                { "TotalSizeBytes", totalSize },
                { "UsedSpaceBytes", usedSpace },
                { "AvailableFreeSpaceBytes", availableFreeSpace },
                { "UsagePercentage", Math.Round(usagePercentage, 2) }
            };

            var status = availableFreeSpace > _thresholdBytes && usagePercentage < 90
                ? HealthStatus.Healthy
                : HealthStatus.Degraded;

            return Task.FromResult(new HealthCheckResult(
                status,
                description: $"Disk usage: {usagePercentage:F1}% ({availableFreeSpace / 1024 / 1024 / 1024}GB free)",
                data: data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new HealthCheckResult(
                HealthStatus.Unhealthy,
                description: $"Failed to check disk space: {ex.Message}"));
        }
    }
}

public class CpuHealthCheck : IHealthCheck
{
    private readonly double _thresholdPercentage;

    public CpuHealthCheck(double thresholdPercentage = 80)
    {
        _thresholdPercentage = thresholdPercentage;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var process = Process.GetCurrentProcess();
            var cpuUsage = process.TotalProcessorTime.TotalMilliseconds / Environment.TickCount * 100;
            var threadCount = process.Threads.Count;
            var handleCount = process.HandleCount;

            var data = new Dictionary<string, object>
            {
                { "CpuUsagePercentage", Math.Round(cpuUsage, 2) },
                { "ThreadCount", threadCount },
                { "HandleCount", handleCount },
                { "ProcessId", process.Id }
            };

            var status = cpuUsage < _thresholdPercentage
                ? HealthStatus.Healthy
                : HealthStatus.Degraded;

            return Task.FromResult(new HealthCheckResult(
                status,
                description: $"CPU usage: {cpuUsage:F1}%, Threads: {threadCount}",
                data: data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new HealthCheckResult(
                HealthStatus.Unhealthy,
                description: $"Failed to check CPU usage: {ex.Message}"));
        }
    }
}

public class ApplicationHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var data = new Dictionary<string, object>
        {
            { "MachineName", Environment.MachineName },
            { "OSVersion", Environment.OSVersion.ToString() },
            { "ProcessorCount", Environment.ProcessorCount },
            { "CLRVersion", Environment.Version.ToString() },
            { "WorkingSet", Environment.WorkingSet / 1024 / 1024 },
            { "StartTime", Process.GetCurrentProcess().StartTime.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        return Task.FromResult(new HealthCheckResult(
            HealthStatus.Healthy,
            description: "Application is running",
            data: data));
    }
}
