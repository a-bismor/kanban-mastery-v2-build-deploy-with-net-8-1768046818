using KanbanApi.Models;

namespace KanbanApi.Services;

public interface IBoardService
{
    Task<Board> CreateAsync(string name, CancellationToken ct = default);
    Task<Board?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Board>> ListAsync(CancellationToken ct = default);
    Task<bool> UpdateNameAsync(int id, string name, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
