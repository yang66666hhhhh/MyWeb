using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication1.Features.DailyPlans;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Shared.Data.Configurations;

public class DailyPlanConfiguration : IEntityTypeConfiguration<DailyPlan>
{
    public void Configure(EntityTypeBuilder<DailyPlan> entity)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        entity.ToTable("DailyPlans");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.PlanDate).HasColumnType("date").HasConversion(dateOnlyConverter).IsRequired();
        entity.Property(x => x.Title).HasMaxLength(120).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(1000);
        entity.Property(x => x.Priority).HasDefaultValue(3);
        entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(DailyPlanStatus.Pending);
        entity.Property(x => x.Remark).HasMaxLength(1000);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.UserId);
        entity.HasIndex(x => x.PlanDate);
        entity.HasIndex(x => x.Status);
    }
}
