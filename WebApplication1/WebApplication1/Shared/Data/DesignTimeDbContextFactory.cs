using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebApplication1.Shared.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = "Server=localhost;Port=3306;Database=personal_growth;User=root;Password=123456;CharSet=utf8mb4;";
        optionsBuilder.UseMySQL(connectionString);
        return new AppDbContext(optionsBuilder.Options);
    }
}
