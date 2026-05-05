using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Features.Auth.Entities;

namespace WebApplication1.Shared.Data.Configurations;

public class RoleMenuConfiguration : IEntityTypeConfiguration<RoleMenu>
{
    public void Configure(EntityTypeBuilder<RoleMenu> entity)
    {
        entity.ToTable("RoleMenus");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        entity.Property(x => x.Path).HasMaxLength(200).IsRequired();
        entity.Property(x => x.Icon).HasMaxLength(100);
        entity.Property(x => x.Component).HasMaxLength(200);
        entity.Property(x => x.Permission).HasMaxLength(100);
        entity.Property(x => x.Redirect).HasMaxLength(200);
        entity.Property(x => x.Badge).HasMaxLength(50);
        entity.Property(x => x.Tag).HasMaxLength(50);
        entity.Property(x => x.PersonaTag).HasMaxLength(50);
        entity.Property(x => x.MenuCategory).HasMaxLength(50).HasDefaultValue("General");
        entity.Property(x => x.FeatureCode).HasMaxLength(100);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.Property(x => x.UpdatedAt).HasColumnType("datetime");
        entity.Property(x => x.DeletedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(x => x.PersonaTag);
        entity.HasIndex(x => x.MinRoleLevel);
        entity.HasIndex(x => x.MenuCategory);
        entity.HasIndex(x => x.FeatureCode);
    }
}