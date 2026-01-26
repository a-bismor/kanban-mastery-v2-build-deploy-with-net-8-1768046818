using System;

namespace KanbanApi.Models;

public class BoardMember
{
    public int BoardId { get; set; }
    public Board Board { get; set; } = null!;
    public string UserId { get; set; } = "";
    public ApplicationUser User { get; set; } = null!;
    public string Role { get; set; } = "Member";
}
