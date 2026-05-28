using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Network.Dtos;
using WebApplication1.Features.Network.Entities;
using WebApplication1.Features.Network.Services.Interfaces;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Network.Services;

public class NetworkService(AppDbContext context) : INetworkService
{
    public async Task<PageResult<ContactDto>> GetContactPageAsync(NetworkQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var contacts = context.Contacts.AsNoTracking();

        if (userId.HasValue)
        {
            contacts = contacts.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            contacts = contacts.Where(x => x.Name.Contains(keyword) || 
                                          (x.Company != null && x.Company.Contains(keyword)) ||
                                          (x.Phone != null && x.Phone.Contains(keyword)) ||
                                          (x.Email != null && x.Email.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.Tag))
        {
            var tag = query.Tag.Trim();
            contacts = contacts.Where(x => x.Tags != null && x.Tags.Contains(tag));
        }

        var total = await contacts.CountAsync(cancellationToken);
        var items = await contacts
            .OrderByDescending(x => x.LastInteractionAt)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<ContactDto>.Create(items.Select(ToContactDto).ToList(), total, page, pageSize);
    }

    public async Task<ContactDto?> GetContactByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contact = await context.Contacts.FindAsync(id, cancellationToken);
        return contact is null ? null : ToContactDto(contact);
    }

    public async Task<ContactDto> CreateContactAsync(CreateContactDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var contact = new Contact
        {
            UserId = userId,
            Name = input.Name,
            Company = input.Company,
            Position = input.Position,
            Phone = input.Phone,
            Email = input.Email,
            WeChat = input.WeChat,
            Tags = input.Tags,
            Remark = input.Remark
        };

        context.Contacts.Add(contact);
        await context.SaveChangesAsync(cancellationToken);

        return ToContactDto(contact);
    }

    public async Task<ContactDto?> UpdateContactAsync(Guid id, UpdateContactDto input, CancellationToken cancellationToken = default)
    {
        var contact = await context.Contacts.FindAsync(id, cancellationToken);
        if (contact is null) return null;

        if (input.Name is not null) contact.Name = input.Name;
        if (input.Company is not null) contact.Company = input.Company;
        if (input.Position is not null) contact.Position = input.Position;
        if (input.Phone is not null) contact.Phone = input.Phone;
        if (input.Email is not null) contact.Email = input.Email;
        if (input.WeChat is not null) contact.WeChat = input.WeChat;
        if (input.Tags is not null) contact.Tags = input.Tags;
        if (input.Remark is not null) contact.Remark = input.Remark;

        await context.SaveChangesAsync(cancellationToken);
        return ToContactDto(contact);
    }

    public async Task<bool> DeleteContactAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var contact = await context.Contacts.FindAsync(id, cancellationToken);
        if (contact is null) return false;

        // 同时删除关联的互动记录
        var interactions = await context.Interactions
            .Where(x => x.ContactId == id)
            .ToListAsync(cancellationToken);
        context.Interactions.RemoveRange(interactions);

        context.Contacts.Remove(contact);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PageResult<InteractionDto>> GetInteractionPageAsync(Guid contactId, NetworkQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var interactions = context.Interactions
            .AsNoTracking()
            .Include(x => x.Contact)
            .Where(x => x.ContactId == contactId);

        if (userId.HasValue)
        {
            interactions = interactions.Where(x => x.UserId == userId.Value);
        }

        var total = await interactions.CountAsync(cancellationToken);
        var items = await interactions
            .OrderByDescending(x => x.InteractionDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<InteractionDto>.Create(items.Select(ToInteractionDto).ToList(), total, page, pageSize);
    }

    public async Task<InteractionDto?> GetInteractionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var interaction = await context.Interactions
            .Include(x => x.Contact)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return interaction is null ? null : ToInteractionDto(interaction);
    }

    public async Task<InteractionDto> CreateInteractionAsync(CreateInteractionDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var interaction = new Interaction
        {
            UserId = userId,
            ContactId = input.ContactId,
            Type = input.Type,
            Content = input.Content,
            InteractionDate = input.InteractionDate,
            NextFollowUpDate = input.NextFollowUpDate,
            Remark = input.Remark
        };

        context.Interactions.Add(interaction);

        // 更新联系人的互动次数和最后互动时间
        var contact = await context.Contacts.FindAsync(input.ContactId, cancellationToken);
        if (contact != null)
        {
            contact.InteractionCount++;
            contact.LastInteractionAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);

        interaction.Contact = contact;
        return ToInteractionDto(interaction);
    }

    public async Task<InteractionDto?> UpdateInteractionAsync(Guid id, UpdateInteractionDto input, CancellationToken cancellationToken = default)
    {
        var interaction = await context.Interactions
            .Include(x => x.Contact)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (interaction is null) return null;

        if (input.Type is not null) interaction.Type = input.Type;
        if (input.Content is not null) interaction.Content = input.Content;
        if (input.InteractionDate is not null) interaction.InteractionDate = input.InteractionDate;
        if (input.NextFollowUpDate is not null) interaction.NextFollowUpDate = input.NextFollowUpDate;
        if (input.Remark is not null) interaction.Remark = input.Remark;

        await context.SaveChangesAsync(cancellationToken);
        return ToInteractionDto(interaction);
    }

    public async Task<bool> DeleteInteractionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var interaction = await context.Interactions.FindAsync(id, cancellationToken);
        if (interaction is null) return false;

        // 更新联系人的互动次数
        var contact = await context.Contacts.FindAsync(interaction.ContactId, cancellationToken);
        if (contact != null && contact.InteractionCount > 0)
        {
            contact.InteractionCount--;
        }

        context.Interactions.Remove(interaction);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<TagDto>> GetContactTagsAsync(Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var contacts = context.Contacts.AsNoTracking();

        if (userId.HasValue)
        {
            contacts = contacts.Where(x => x.UserId == userId.Value);
        }

        var allTags = await contacts
            .Where(x => x.Tags != null)
            .Select(x => x.Tags!)
            .ToListAsync(cancellationToken);

        var tagCounts = allTags
            .SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .GroupBy(x => x.Trim())
            .Select(g => new TagDto
            {
                Id = g.Key,
                Name = g.Key,
                Color = "#1890ff",
                UsageCount = g.Count()
            })
            .OrderByDescending(x => x.UsageCount)
            .ToList();

        return tagCounts;
    }

    private static ContactDto ToContactDto(Contact contact) => new()
    {
        Id = contact.Id.ToString(),
        UserId = contact.UserId?.ToString(),
        Name = contact.Name,
        Company = contact.Company,
        Position = contact.Position,
        Phone = contact.Phone,
        Email = contact.Email,
        WeChat = contact.WeChat,
        Tags = contact.Tags,
        Remark = contact.Remark,
        InteractionCount = contact.InteractionCount,
        LastInteractionAt = contact.LastInteractionAt?.ToString("yyyy-MM-dd HH:mm:ss"),
        CreatedAt = contact.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
    };

    private static InteractionDto ToInteractionDto(Interaction interaction) => new()
    {
        Id = interaction.Id.ToString(),
        UserId = interaction.UserId?.ToString(),
        ContactId = interaction.ContactId.ToString(),
        ContactName = interaction.Contact?.Name ?? string.Empty,
        Type = interaction.Type,
        Content = interaction.Content,
        InteractionDate = interaction.InteractionDate,
        NextFollowUpDate = interaction.NextFollowUpDate,
        Remark = interaction.Remark,
        CreatedAt = interaction.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
    };
}
