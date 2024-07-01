using System.ComponentModel.DataAnnotations;
using Domain.Abstractions;

namespace Domain.Models;

public class Channel : Entity
{
    [MaxLength(100)] public string Name { get; init; } = null!;
    [MaxLength(5)] public string Type { get; init; } = null!;
    public int Position { get; init; }

    public List<RoleOverwrite> RoleOverwrites { get; } = [];
    public List<Message> Messages { get; } = [];

    public int CategoryId { get; init; }
    public Category Category { get; init; } = null!;
}

public enum ChannelType
{
    Text,
    Voice,
    Forum,
    News,
    Stage
}