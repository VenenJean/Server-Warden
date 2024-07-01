using Domain.Abstractions;

namespace Domain.Models;

public class Member : Entity
{
    public bool IsApproved { get; init; }
}