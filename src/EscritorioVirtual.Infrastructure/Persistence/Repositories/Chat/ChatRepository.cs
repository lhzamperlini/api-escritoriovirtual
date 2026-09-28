using EscritorioVirtual.Application.Chat.Interfaces;
using EscritorioVirtual.Domain.AggregateRoot.Chat;
using EscritorioVirtual.Domain.Aggregates.Chat;
using EscritorioVirtual.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EscritorioVirtual.Infrastructure.Persistence.Repositories.Chat;

public class ChatRepository(AppDbContext appDbContext)
    : BaseRepository<ChatChannel>(appDbContext), IChatRepository
{
    private DbSet<ChatMessage> Messages => AppDbContext.Set<ChatMessage>();
    private DbSet<ChatChannelMember> ChannelMembers => AppDbContext.Set<ChatChannelMember>();

    public async Task<List<ChatChannel>> GetWorkspaceChannelsAsync(Guid workspaceId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Members)
            .Where(c => c.WorkspaceId == workspaceId &&
                        (c.ChannelType == "Global" ||
                         c.ChannelType == "Zone" ||
                         (c.ChannelType == "Direct" && c.Members.Any(m => m.UserId == userId))))
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ChatChannel?> GetChannelByIdAsync(Guid channelId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == channelId, cancellationToken);
    }

    public async Task<ChatChannel> GetOrCreateGlobalChannelAsync(Guid workspaceId, CancellationToken cancellationToken = default)
    {
        var channel = await DbSet
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.WorkspaceId == workspaceId && c.ChannelType == "Global", cancellationToken);

        if (channel is null)
        {
            channel = new ChatChannel(workspaceId, "Global", "#geral");
            await DbSet.AddAsync(channel, cancellationToken);
            await AppDbContext.SaveChangesAsync(cancellationToken);
        }

        return channel;
    }

    public async Task<ChatChannel> GetOrCreateZoneChannelAsync(Guid workspaceId, Guid zoneId, string zoneName, CancellationToken cancellationToken = default)
    {
        var channel = await DbSet
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.WorkspaceId == workspaceId && c.ZoneId == zoneId, cancellationToken);

        if (channel is null)
        {
            channel = new ChatChannel(workspaceId, "Zone", $"#{zoneName}", zoneId);
            await DbSet.AddAsync(channel, cancellationToken);
            await AppDbContext.SaveChangesAsync(cancellationToken);
        }

        return channel;
    }

    public async Task<ChatChannel> GetOrCreateDirectChannelAsync(Guid workspaceId, Guid userA, Guid userB, string targetUserName, CancellationToken cancellationToken = default)
    {
        var channels = await DbSet
            .Include(c => c.Members)
            .Where(c => c.WorkspaceId == workspaceId && c.ChannelType == "Direct")
            .ToListAsync(cancellationToken);

        var existing = channels.FirstOrDefault(c =>
            c.Members.Any(m => m.UserId == userA) && c.Members.Any(m => m.UserId == userB));

        if (existing is not null)
        {
            return existing;
        }

        var direct = new ChatChannel(workspaceId, "Direct", targetUserName);
        direct.AddMember(userA);
        direct.AddMember(userB);

        await DbSet.AddAsync(direct, cancellationToken);
        await AppDbContext.SaveChangesAsync(cancellationToken);

        return direct;
    }

    public async Task<List<ChatMessage>> GetChannelMessagesAsync(Guid channelId, int take = 50, CancellationToken cancellationToken = default)
    {
        return await Messages
            .AsNoTracking()
            .Where(m => m.ChannelId == channelId)
            .OrderByDescending(m => m.CreatedAt)
            .Take(take)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddMessageAsync(ChatMessage message, CancellationToken cancellationToken = default)
    {
        await Messages.AddAsync(message, cancellationToken);
        await AppDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkChannelAsReadAsync(Guid channelId, Guid userId, CancellationToken cancellationToken = default)
    {
        var member = await ChannelMembers
            .FirstOrDefaultAsync(m => m.ChannelId == channelId && m.UserId == userId, cancellationToken);

        if (member is not null)
        {
            member.MarkRead();
            await AppDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
