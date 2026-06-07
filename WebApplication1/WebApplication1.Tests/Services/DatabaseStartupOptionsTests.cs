using Microsoft.Extensions.Configuration;
using WebApplication1.Shared.Configuration;

namespace WebApplication1.Tests.Services;

public class DatabaseStartupOptionsTests
{
    [Fact]
    public void FromConfiguration_ShouldDisableMigrationAndSeedingByDefault()
    {
        var configuration = new ConfigurationBuilder().Build();

        var options = DatabaseStartupOptions.FromConfiguration(configuration);

        Assert.False(options.AutoMigrate);
        Assert.False(options.SeedOnStartup);
    }

    [Fact]
    public void FromConfiguration_ShouldReadDatabaseStartupSwitches()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:AutoMigrate"] = "true",
                ["Database:SeedOnStartup"] = "true"
            })
            .Build();

        var options = DatabaseStartupOptions.FromConfiguration(configuration);

        Assert.True(options.AutoMigrate);
        Assert.True(options.SeedOnStartup);
    }

    [Fact]
    public void ResolveConnectionString_ShouldUseEnvironmentValue_WhenConfiguredConnectionStringIsEmpty()
    {
        const string environmentConnection = "Server=mysql;Database=personal_growth;";
        Environment.SetEnvironmentVariable("DB_CONNECTION", environmentConnection);

        try
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = ""
                })
                .Build();

            var connectionString = DatabaseStartupOptions.ResolveConnectionString(configuration);

            Assert.Equal(environmentConnection, connectionString);
        }
        finally
        {
            Environment.SetEnvironmentVariable("DB_CONNECTION", null);
        }
    }

    [Fact]
    public void ResolveJwtSecretKey_ShouldUseEnvironmentValue_WhenConfiguredSecretIsEmpty()
    {
        const string environmentSecret = "environment-secret-key-with-more-than-32-characters";
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", environmentSecret);

        try
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:SecretKey"] = ""
                })
                .Build();

            var jwtSecretKey = DatabaseStartupOptions.ResolveJwtSecretKey(configuration);

            Assert.Equal(environmentSecret, jwtSecretKey);
        }
        finally
        {
            Environment.SetEnvironmentVariable("JWT_SECRET_KEY", null);
        }
    }
}
