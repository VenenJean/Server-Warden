using System.ComponentModel.DataAnnotations;
using Domain.Abstractions;

namespace Domain.Models;

public class Message : Entity
{
    [MaxLength(2000)] public string Content { get; init; } = null!;

    public int ChannelId { get; init; }
    public DateTimeOffset SentTime { get; init; }
    public Channel Channel { get; init; } = null!;
}