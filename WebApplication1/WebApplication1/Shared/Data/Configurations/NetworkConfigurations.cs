using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Features.Network.Entities;

namespace WebApplication1.Shared.Data.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Company)
            .HasMaxLength(200);

        builder.Property(x => x.Position)
            .HasMaxLength(100);

        builder.Property(x => x.Phone)
            .HasMaxLength(20);

        builder.Property(x => x.Email)
            .HasMaxLength(100);

        builder.Property(x => x.WeChat)
            .HasMaxLength(50);

        builder.Property(x => x.Tags)
            .HasMaxLength(500);

        builder.Property(x => x.Remark)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.Company);
        builder.HasIndex(x => x.Phone);
        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.Tags);
        builder.HasIndex(x => x.LastInteractionAt);
        builder.HasIndex(x => new { x.UserId, x.Name });
    }
}

public class InteractionConfiguration : IEntityTypeConfiguration<Interaction>
{
    public void Configure(EntityTypeBuilder<Interaction> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.InteractionDate)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.NextFollowUpDate)
            .HasMaxLength(20);

        builder.Property(x => x.Remark)
            .HasMaxLength(1000);

        builder.HasOne(x => x.Contact)
            .WithMany()
            .HasForeignKey(x => x.ContactId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.ContactId);
        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => x.InteractionDate);
        builder.HasIndex(x => new { x.ContactId, x.InteractionDate });
    }
}
