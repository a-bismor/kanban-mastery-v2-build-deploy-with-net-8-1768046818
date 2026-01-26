using System.Collections.Generic;
namespace KanbanApi.Models;

public class Board
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public ICollection<Column> Columns { get; set; } = new List<Column>();
    public ICollection<BoardMember> Members { get; set; } = new List<BoardMember>();

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

