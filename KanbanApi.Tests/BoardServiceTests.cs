using System.Net.Http.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using KanbanApi.Data;
using KanbanApi.Models;
using KanbanApi.Services;
using Xunit;

namespace KanbanApi.Tests;

public class BoardServiceTests
{
    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(CreateOpenConnection())
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private static SqliteConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        return connection;
    }

    [Fact]
    public async Task CreateAsync_throws_when_name_is_empty()
    {
        await using var db = CreateDbContext();
        IBoardService service = new BoardService(db);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(""));

        Assert.Equal("name", ex.ParamName);
    }

    [Fact]
    public async Task UpdateNameAsync_throws_when_name_is_empty()
    {
        await using var db = CreateDbContext();
        IBoardService service = new BoardService(db);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateNameAsync(1, " "));

        Assert.Equal("name", ex.ParamName);
    }

    [Fact]
    public async Task UpdateNameAsync_returns_false_when_board_missing()
    {
        await using var db = CreateDbContext();
        IBoardService service = new BoardService(db);

        var updated = await service.UpdateNameAsync(999, "New Name");

        Assert.False(updated);
    }

    [Fact]
    public async Task CreateAsync_creates_board_with_default_columns()
    {
        await using var db = CreateDbContext();
        IBoardService service = new BoardService(db);

        var board = await service.CreateAsync("Sprint 1");

        Assert.Equal("Sprint 1", board.Name);
        Assert.Equal(4, board.Columns.Count);
    }

    [Fact]
    public async Task GetByIdAsync_returns_null_when_missing()
    {
        await using var db = CreateDbContext();
        IBoardService service = new BoardService(db);

        var board = await service.GetByIdAsync(123);

        Assert.Null(board);
    }

    [Fact]
    public async Task ListAsync_returns_all_boards()
    {
        await using var db = CreateDbContext();
        IBoardService service = new BoardService(db);

        await service.CreateAsync("Board A");
        await service.CreateAsync("Board B");

        var boards = await service.ListAsync();

        Assert.Equal(2, boards.Count);
    }

    [Fact]
    public async Task DeleteAsync_returns_false_when_missing()
    {
        await using var db = CreateDbContext();
        IBoardService service = new BoardService(db);

        var deleted = await service.DeleteAsync(555);

        Assert.False(deleted);
    }

    [Fact]
    public async Task DeleteAsync_removes_existing_board()
    {
        await using var db = CreateDbContext();
        IBoardService service = new BoardService(db);

        var board = await service.CreateAsync("Board X");

        var deleted = await service.DeleteAsync(board.Id);
        var fetched = await service.GetByIdAsync(board.Id);

        Assert.True(deleted);
        Assert.Null(fetched);
    }
}
