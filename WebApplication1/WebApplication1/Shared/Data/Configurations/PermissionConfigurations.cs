using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Features.Auth.Entities;

namespace WebApplication1.Shared.Data.Configurations;

public class UserPersonaConfiguration : IEntityTypeConfiguration<UserPersona>
{
    public void Configure(EntityTypeBuilder<UserPersona> entity)
    {
        entity.ToTable("UserPersonas");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.IsPrimary).HasDefaultValue(false);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.User)
            .WithMany(x => x.UserPersonas)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.PersonaType)
            .WithMany()
            .HasForeignKey(x => x.PersonaTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => new { x.UserId, x.PersonaTypeId }).IsUnique();
    }
}

public class MenuActionConfiguration : IEntityTypeConfiguration<MenuAction>
{
    public void Configure(EntityTypeBuilder<MenuAction> entity)
    {
        entity.ToTable("MenuActions");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.ActionCode).HasMaxLength(50).IsRequired();
        entity.Property(x => x.ActionName).HasMaxLength(100).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Menu)
            .WithMany(x => x.Actions)
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => new { x.MenuId, x.ActionCode }).IsUnique();
    }
}

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> entity)
    {
        entity.ToTable("RolePermissions");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.ActionCode).HasMaxLength(50).IsRequired();
        entity.Property(x => x.IsAllowed).HasDefaultValue(true);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.Menu)
            .WithMany()
            .HasForeignKey(x => x.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => new { x.RoleId, x.MenuId, x.ActionCode }).IsUnique();
    }
}
