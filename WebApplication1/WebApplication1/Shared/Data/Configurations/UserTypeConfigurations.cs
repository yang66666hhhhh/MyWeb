using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Features.Auth.Entities;

namespace WebApplication1.Shared.Data.Configurations;

public class UserTypeConfiguration : IEntityTypeConfiguration<UserType>
{
    public void Configure(EntityTypeBuilder<UserType> entity)
    {
        entity.ToTable("UserTypes");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.Property(x => x.Color).HasMaxLength(20);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Code).IsUnique();
    }
}

public class UserTypeTagConfiguration : IEntityTypeConfiguration<UserTypeTag>
{
    public void Configure(EntityTypeBuilder<UserTypeTag> entity)
    {
        entity.ToTable("UserTypeTags");
        entity.HasKey(x => new { x.UserTypeId, x.TagId });
        entity.HasOne(x => x.UserType).WithMany(x => x.UserTypeTags).HasForeignKey(x => x.UserTypeId).OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(x => x.Tag).WithMany().HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Cascade);
    }
}
