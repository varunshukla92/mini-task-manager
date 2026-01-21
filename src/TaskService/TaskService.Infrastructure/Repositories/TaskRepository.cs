using Microsoft.EntityFrameworkCore;
using TaskService.Application.Interfaces;
using TaskService.Domain.Entities;
using TaskService.Infrastructure.Data;

namespace TaskService.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _dbContext;

    public TaskRepository(TaskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(TaskItem task, CancellationToken ct)
    {
        await _dbContext.Tasks.AddAsync(task, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid taskId, Guid userId, CancellationToken ct)
    {
        return await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId, ct);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(Guid userId, CancellationToken ct)
    {
        return await _dbContext.Tasks
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken ct)
    {
        _dbContext.Tasks.Update(task);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(TaskItem task, CancellationToken ct)
    {
        _dbContext.Tasks.Remove(task);
        await _dbContext.SaveChangesAsync(ct);
    }

    
}
