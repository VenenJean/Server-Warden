using System.ComponentModel.DataAnnotations;
using Domain.Abstractions;

namespace Domain.Models;

public class Role : Entity
{
    [MaxLength(100)] public string Name { get; init; } = null!;
    public uint Color { get; init; }
    public int Position { get; init; }
    public bool IsHoisted { get; init; }
    public bool IsMentionable { get; init; }
    public ulong BitwisePermissionFlag { get; init; }

    public int GuildId { get; init; }
    public Guild Guild { get; init; } = null!;
}