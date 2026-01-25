using System.Collections.Generic;

public class Board
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
<<<<<<< HEAD
    // TODO: Consider Using ICollecion instead of List as an interface keeps the property less coupled to a specific implementation.
=======
>>>>>>> e8d1ac6 (feat(models): add Board, Column, Card, ApplicationUser and BoardMember)
    public List<Column> Columns { get; set; } = new();
    public List<BoardMember> Members { get; set; } = new();

    // Factory function
    public static Board Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Board name cannot be empty.", nameof(name));

        return new Board
        {
            Name = name,
            Columns = new List<Column>
            {
                new Column { Name = "To Do",       Order = 1 },
                new Column { Name = "In Progress", Order = 2 },
                new Column { Name = "Testing",     Order = 3 },
                new Column { Name = "Done",        Order = 4 },
            }
        };
    }
}

