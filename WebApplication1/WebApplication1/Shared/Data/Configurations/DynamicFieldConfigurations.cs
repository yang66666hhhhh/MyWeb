using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Shared.Data.Configurations;

public class IndustryTemplateConfiguration : IEntityTypeConfiguration<IndustryTemplate>
{
    public void Configure(EntityTypeBuilder<IndustryTemplate> entity)
    {
        entity.ToTable("IndustryTemplates");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.Property(x => x.Industry).HasMaxLength(50).IsRequired();
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Name).IsUnique();
    }
}

public class TemplateFieldConfiguration : IEntityTypeConfiguration<TemplateField>
{
    public void Configure(EntityTypeBuilder<TemplateField> entity)
    {
        entity.ToTable("TemplateFields");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.FieldName).HasMaxLength(50).IsRequired();
        entity.Property(x => x.FieldLabel).HasMaxLength(100).IsRequired();
        entity.Property(x => x.FieldType).HasConversion<int>().HasDefaultValue(FieldType.Text);
        entity.Property(x => x.Options).HasMaxLength(1000);
        entity.Property(x => x.DefaultValue).HasMaxLength(500);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Template)
            .WithMany(x => x.Fields)
            .HasForeignKey(x => x.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => new { x.TemplateId, x.FieldName }).IsUnique();
    }
}

public class WorkLogDynamicValueConfiguration : IEntityTypeConfiguration<WorkLogDynamicValue>
{
    public void Configure(EntityTypeBuilder<WorkLogDynamicValue> entity)
    {
        entity.ToTable("WorkLogDynamicValues");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.FieldName).HasMaxLength(50).IsRequired();
        entity.Property(x => x.StringValue).HasMaxLength(1000);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.WorkLog)
            .WithMany(x => x.DynamicValues)
            .HasForeignKey(x => x.WorkLogId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => new { x.WorkLogId, x.FieldName }).IsUnique();
    }
}
