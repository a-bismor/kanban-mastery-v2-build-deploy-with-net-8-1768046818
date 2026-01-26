public class Card
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public int Order { get; set; }     // Ordering inside the column
    // TODO: consider setting a default value (e.g. DateTime.UtcNow) to ensure it is always populated
    public DateTime CreatedAt { get; set; }
    public int ColumnId { get; set; }
    public Column Column { get; set; } = null!;
}
