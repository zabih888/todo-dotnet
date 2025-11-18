using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Dtos;
using TodoApi.Exceptions;
using TodoApi.Models;

namespace TodoApi.Services;

public class TodoService
{
    private readonly AppDbContext _db;

    public TodoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Todo>> GetAllAsync()
    {
        return await _db.Todos.ToListAsync();
    }

    public async Task<List<Todo>> GetFilteredAsync(bool? completed, string? search)
    {
        var query = _db.Todos.AsQueryable();

        if (completed is not null)
        {
            query = query.Where(t => t.IsCompleted == completed);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => EF.Functions.ILike(t.Title, $"%{search}%"));
        }

        return await query.ToListAsync();
    }

    public async Task<Todo?> GetTodoAsync(int id)
    {
        return await _db.Todos.FindAsync(id);
    }

    public async Task<Todo> CreateAsync(TodoCreateDto dto)
    {
        var count = await _db.Todos.CountAsync();
        if (count >= 100)
            throw new BusinessRuleException("You reached the maximum todo limit of 100.");

        var todo = new Todo
        {
            Title = dto.Title,
            IsCompleted = dto.IsCompleted,
            CreatedAt = DateTime.UtcNow
        };

        _db.Todos.Add(todo);
        await _db.SaveChangesAsync();

        return todo;
    }

    public async Task<Todo?> UpdateAsync(int id, TodoUpdateDto dto)
    {
        var todo = await _db.Todos.FindAsync(id);
        if (todo is null) return null;

        todo.Title = dto.Title;
        todo.IsCompleted = dto.IsCompleted;

        await _db.SaveChangesAsync();
        return todo;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var todo = await _db.Todos.FindAsync(id);
        if (todo is null) return false;

        _db.Todos.Remove(todo);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Todo>> GetAllCompletedAsync()
    {
        return await _db.Todos.Where(t => t.IsCompleted).ToListAsync();
    }

    public async Task<List<Todo>> GetAllPendingAsync()
    {
        return await _db.Todos.Where(t => !t.IsCompleted).ToListAsync();
    }
}
