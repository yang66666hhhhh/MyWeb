using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Features.Auth.Entities.Subscription;

namespace WebApplication1.Shared.Data.Configurations;

public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
{
    public void Configure(EntityTypeBuilder<Feature> entity)
    {
        entity.ToTable("Features");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Code).HasMaxLength(100).IsRequired();
        entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.Property(x => x.Category).HasMaxLength(50).IsRequired();
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Code).IsUnique();
    }
}

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> entity)
    {
        entity.ToTable("Plans");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.Property(x => x.Price).HasPrecision(10, 2);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Code).IsUnique();
    }
}

public class PlanFeatureConfiguration : IEntityTypeConfiguration<PlanFeature>
{
    public void Configure(EntityTypeBuilder<PlanFeature> entity)
    {
        entity.ToTable("PlanFeatures");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Plan)
            .WithMany(x => x.PlanFeatures)
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.Feature)
            .WithMany()
            .HasForeignKey(x => x.FeatureId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => new { x.PlanId, x.FeatureId }).IsUnique();
    }
}

public class PersonaFeatureConfiguration : IEntityTypeConfiguration<PersonaFeature>
{
    public void Configure(EntityTypeBuilder<PersonaFeature> entity)
    {
        entity.ToTable("PersonaFeatures");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.PersonaCode).HasMaxLength(50).IsRequired();
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Feature)
            .WithMany()
            .HasForeignKey(x => x.FeatureId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => new { x.PersonaCode, x.FeatureId }).IsUnique();
    }
}

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> entity)
    {
        entity.ToTable("UserSubscriptions");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.StartAt).HasColumnType("datetime");
        entity.Property(x => x.ExpireAt).HasColumnType("datetime");
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Plan)
            .WithMany()
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => x.UserId);
    }
}
