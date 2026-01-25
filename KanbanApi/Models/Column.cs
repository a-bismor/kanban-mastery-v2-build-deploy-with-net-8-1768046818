using System.Collections.Generic;

public class Column
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int Order { get; set; }
    public int BoardId { get; set; }
    public Board Board { get; set; } = null!;
<<<<<<< HEAD
    // TODO: Consider Using ICollecion instead of List as an interface keeps the property less coupled to a specific implementation.
=======
>>>>>>> e8d1ac6 (feat(models): add Board, Column, Card, ApplicationUser and BoardMember)
    public List<Card> Cards { get; set; } = new();
}
