namespace WebApplication1.Shared.Configuration;

public sealed class DatabaseStartupOptions
{
    public const string SectionName = "Database";

    public bool AutoMigrate { get; init; }

    public bool SeedOnStartup { get; init; }

    public static DatabaseStartupOptions FromConfiguration(IConfiguration configuration)
    {
        return configuration
            .GetSection(SectionName)
            .Get<DatabaseStartupOptions>()
            ?? new DatabaseStartupOptions();
    }

    public static string ResolveConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString;
        }

        connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString;
        }

        throw new InvalidOperationException("DefaultConnection or DB_CONNECTION is not configured.");
    }

    public static string ResolveJwtSecretKey(IConfiguration configuration)
    {
        var jwtSecretKey = configuration["Jwt:SecretKey"];
        if (!string.IsNullOrWhiteSpace(jwtSecretKey))
        {
            return jwtSecretKey;
        }

        jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
        if (!string.IsNullOrWhiteSpace(jwtSecretKey))
        {
            return jwtSecretKey;
        }

        throw new InvalidOperationException("Jwt:SecretKey or JWT_SECRET_KEY is not configured.");
    }
}
