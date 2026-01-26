namespace KanbanApi.Models;

public class Card
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int ColumnId { get; set; }
    public Column Column { get; set; } = null!;
}
