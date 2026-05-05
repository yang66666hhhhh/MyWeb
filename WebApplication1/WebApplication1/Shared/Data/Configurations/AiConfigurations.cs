using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Features.Ai.Entities;

namespace WebApplication1.Shared.Data.Configurations;

public class AiPlanConfiguration : IEntityTypeConfiguration<AiPlan>
{
    public void Configure(EntityTypeBuilder<AiPlan> entity)
    {
        entity.ToTable("AiPlans");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(1000);
        entity.Property(x => x.Type).HasConversion<int>();
        entity.Property(x => x.Status).HasConversion<int>();
        entity.Property(x => x.GeneratedContent).HasMaxLength(10000);
        entity.Property(x => x.Remark).HasMaxLength(1000);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasIndex(x => x.UserId);
        entity.HasIndex(x => x.Status);
        entity.HasIndex(x => x.Type);
    }
}

public class AiReportConfiguration : IEntityTypeConfiguration<AiReport>
{
    public void Configure(EntityTypeBuilder<AiReport> entity)
    {
        entity.ToTable("AiReports");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
        entity.Property(x => x.Type).HasConversion<int>();
        entity.Property(x => x.Content).HasMaxLength(10000);
        entity.Property(x => x.Remark).HasMaxLength(1000);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasIndex(x => x.UserId);
        entity.HasIndex(x => x.Type);
    }
}

public class AiChatSessionConfiguration : IEntityTypeConfiguration<AiChatSession>
{
    public void Configure(EntityTypeBuilder<AiChatSession> entity)
    {
        entity.ToTable("AiChatSessions");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
        entity.Property(x => x.LastMessage).HasMaxLength(500);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasIndex(x => x.UserId);
    }
}

public class AiChatMessageConfiguration : IEntityTypeConfiguration<AiChatMessage>
{
    public void Configure(EntityTypeBuilder<AiChatMessage> entity)
    {
        entity.ToTable("AiChatMessages");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Role).HasConversion<int>();
        entity.Property(x => x.Content).HasMaxLength(4000).IsRequired();
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Session)
            .WithMany()
            .HasForeignKey(x => x.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => x.SessionId);
    }
}
