using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Shared.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> entity)
    {
        entity.ToTable("Tenants");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Code).IsUnique();
    }
}

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> entity)
    {
        entity.ToTable("Users");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.TenantId);
        entity.Property(x => x.Username).HasMaxLength(50).IsRequired();
        entity.Property(x => x.PasswordHash).HasMaxLength(100).IsRequired();
        entity.Property(x => x.RealName).HasMaxLength(100).IsRequired();
        entity.Property(x => x.Avatar).HasMaxLength(500);
        entity.Property(x => x.Email).HasMaxLength(100);
        entity.Property(x => x.Phone).HasMaxLength(20);
        entity.Property(x => x.Roles).HasMaxLength(100).HasDefaultValue("member");
        entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(AppUserStatus.Active);
        entity.Property(x => x.LastLoginIp).HasMaxLength(50);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Tenant)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(x => x.Username).IsUnique();
        entity.HasIndex(x => x.Email);
        entity.HasIndex(x => x.TenantId);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> entity)
    {
        entity.ToTable("Roles");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.Property(x => x.Permissions).HasMaxLength(2000);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Code).IsUnique();
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> entity)
    {
        entity.ToTable("UserRoles");
        entity.HasKey(x => new { x.UserId, x.RoleId });
        entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.ToTable("RefreshTokens");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Token).HasMaxLength(200).IsRequired();
        entity.Property(x => x.CreatedByIp).HasMaxLength(50);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.Property(x => x.ExpiresAt).HasColumnType("datetime");
        entity.Property(x => x.RevokedAt).HasColumnType("datetime");

        entity.HasIndex(x => x.Token);
        entity.HasIndex(x => new { x.UserId, x.Token });
    }
}
