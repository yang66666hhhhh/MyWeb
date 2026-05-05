using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Features.Auth.Entities;

namespace WebApplication1.Shared.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> entity)
    {
        entity.ToTable("Tags");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.Property(x => x.Color).HasMaxLength(20);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Name).IsUnique();
    }
}

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> entity)
    {
        entity.ToTable("MenuItems");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Path).HasMaxLength(200).IsRequired();
        entity.Property(x => x.Icon).HasMaxLength(100);
        entity.Property(x => x.RequiredPermissions).HasMaxLength(500);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Cascade);
        entity.HasIndex(x => x.Path).IsUnique();
    }
}

public class MenuTagConfiguration : IEntityTypeConfiguration<MenuTag>
{
    public void Configure(EntityTypeBuilder<MenuTag> entity)
    {
        entity.ToTable("MenuTags");
        entity.HasKey(x => new { x.MenuItemId, x.TagId });
        entity.HasOne(x => x.MenuItem).WithMany(x => x.MenuTags).HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(x => x.Tag).WithMany(x => x.MenuTags).HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class UserTagConfiguration : IEntityTypeConfiguration<UserTag>
{
    public void Configure(EntityTypeBuilder<UserTag> entity)
    {
        entity.ToTable("UserTags");
        entity.HasKey(x => new { x.UserId, x.TagId });
        entity.HasOne(x => x.User).WithMany(x => x.UserTags).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(x => x.Tag).WithMany(x => x.UserTags).HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class MenuConfigConfiguration : IEntityTypeConfiguration<MenuConfig>
{
    public void Configure(EntityTypeBuilder<MenuConfig> entity)
    {
        entity.ToTable("MenuConfigs");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Path).HasMaxLength(200).IsRequired();
        entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Icon).HasMaxLength(100);
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Path).IsUnique();
    }
}
