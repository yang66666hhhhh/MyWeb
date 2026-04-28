using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<DailyPlan> DailyPlans => Set<DailyPlan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DailyPlan>(entity =>
        {
            entity.ToTable("DailyPlans");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.PlanDate)
                .HasColumnType("date")
                .IsRequired();

            entity.Property(x => x.Title)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(1000);

            entity.Property(x => x.Priority)
                .HasDefaultValue(3);

            entity.Property(x => x.Status)
                .HasConversion<int>()
                .HasDefaultValue(Enums.DailyPlanStatus.Pending);

            entity.Property(x => x.Remark)
                .HasMaxLength(1000);

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(x => x.PlanDate);
            entity.HasIndex(x => x.Status);
        });
    }
}
