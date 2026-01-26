using System;

public class BoardMember
{
    public int BoardId { get; set; }
    public Board Board { get; set; } = null!;
    public string UserId { get; set; } = "";
    public ApplicationUser User { get; set; } = null!;
    // TODO: consider using an enum for the Role property instead of a string to avoid relying on magic strings and to prevent invalid values
    public string Role { get; set; } = "Member";
}
