using System;

public class BoardMember
{
    public int BoardId { get; set; }
    public Board Board { get; set; } = null!;
    public string UserId { get; set; } = "";
    public ApplicationUser User { get; set; } = null!;
<<<<<<< HEAD
    // TODO: consider using an enum for the Role property instead of a string to avoid relying on magic strings and to prevent invalid values
=======
>>>>>>> e8d1ac6 (feat(models): add Board, Column, Card, ApplicationUser and BoardMember)
    public string Role { get; set; } = "Member";
}
