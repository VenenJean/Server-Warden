using System.ComponentModel.DataAnnotations;
using Domain.Abstractions;

namespace Domain.Models;

public class Category : Entity
{
    [MaxLength(100)] public string Name { get; init; } = null!;
    public int Position { get; init; }

    public List<RoleOverwrite> RoleOverwrites { get; } = [];
    public List<Channel> Channels { get; } = [];

    public int GuildId { get; init; }
    public Guild Guild { get; init; } = null!;
}