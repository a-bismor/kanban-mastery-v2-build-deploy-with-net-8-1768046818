using KanbanApi.Data;
using KanbanApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KanbanApi.Services;

public class BoardService : IBoardService
{
    private readonly ApplicationDbContext _db;

    public BoardService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Board> CreateAsync(string name, CancellationToken ct = default)
    {
        var board = Board.Create(name);
        _db.Boards.Add(board);
        await _db.SaveChangesAsync(ct);
        return board;
    }

    public async Task<Board?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Boards
            .Include(b => b.Columns)
            .FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public async Task<IReadOnlyList<Board>> ListAsync(CancellationToken ct = default)
    {
        return await _db.Boards
            .Include(b => b.Columns)
            .OrderBy(b => b.Id)
            .ToListAsync(ct);
    }

    public async Task<bool> UpdateNameAsync(int id, string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Board name cannot be empty.", nameof(name));
        }

        var board = await _db.Boards.FirstOrDefaultAsync(b => b.Id == id, ct);
        if (board == null)
        {
            return false;
        }

        board.Name = name;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool?> DeleteAsync(int id, CancellationToken ct = default)
    {
        var board = await _db.Boards.FirstOrDefaultAsync(b => b.Id == id, ct);
        if (board == null)
        {
            return null;
        }

        _db.Boards.Remove(board);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
