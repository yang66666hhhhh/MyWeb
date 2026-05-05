using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Features.Admin.Entities;

namespace WebApplication1.Shared.Data.Configurations;

public class PersonaTypeConfiguration : IEntityTypeConfiguration<PersonaType>
{
    public void Configure(EntityTypeBuilder<PersonaType> entity)
    {
        entity.ToTable("PersonaTypes");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Icon).HasMaxLength(50);
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.Property(x => x.DefaultHomeRoute).HasMaxLength(200);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasIndex(x => x.Code).IsUnique();
        entity.HasIndex(x => x.IsActive);
    }
}

public class UserPersonaRecordConfiguration : IEntityTypeConfiguration<UserPersonaRecord>
{
    public void Configure(EntityTypeBuilder<UserPersonaRecord> entity)
    {
        entity.ToTable("UserPersonaRecords");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Remark).HasMaxLength(500);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.PersonaType)
            .WithMany(x => x.UserPersonaRecords)
            .HasForeignKey(x => x.PersonaTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(x => x.UserId);
        entity.HasIndex(x => x.PersonaTypeId);
    }
}

public class PersonaMenuItemConfiguration : IEntityTypeConfiguration<PersonaMenuItem>
{
    public void Configure(EntityTypeBuilder<PersonaMenuItem> entity)
    {
        entity.ToTable("PersonaMenuItems");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.MenuPath).HasMaxLength(200).IsRequired();
        entity.Property(x => x.MenuName).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Icon).HasMaxLength(100);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.PersonaType)
            .WithMany(x => x.MenuItems)
            .HasForeignKey(x => x.PersonaTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(x => x.PersonaTypeId);
        entity.HasIndex(x => x.MenuPath);
    }
}
