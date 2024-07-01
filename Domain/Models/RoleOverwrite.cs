using System.ComponentModel.DataAnnotations;
using Domain.Abstractions;

namespace Domain.Models;

public class RoleOverwrite : Entity
{
    [MaxLength(100)] public string RoleName { get; init; } = null!;
    public ulong AllowValue { get; init; }
    public ulong DenyValue { get; init; }

    public int CategoryId { get; init; }
    public Category Category { get; init; } = null!;

    public int? ChannelId { get; init; }
    public Channel? Channel { get; init; }
}