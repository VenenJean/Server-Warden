using Domain.Abstractions;

namespace Domain.Models;

public class MemberRole : Entity
{
    public int MemberId { get; init; }

    public Member Member { get; init; } = null!;
    public int RoleId { get; init; }
    public Role Role { get; init; } = null!;
}