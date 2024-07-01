using System.ComponentModel.DataAnnotations;
using Domain.Abstractions;

namespace Domain.Models;

public class Guild : Entity
{
    [MaxLength(100)] public string Name { get; init; } = null!;
    [MaxLength(120)] public string Description { get; init; } = null!;
    [MaxLength(19)] public string GuildId { get; set; } = null!;
    public List<Category> Categories { get; } = [];
    public List<Role> Roles { get; } = [];
}