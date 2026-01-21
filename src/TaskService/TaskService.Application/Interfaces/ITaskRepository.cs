using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskService.Domain.Entities;

namespace TaskService.Application.Interfaces
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskItem task, CancellationToken ct);
        Task<TaskItem?> GetByIdAsync(Guid taskId, Guid userId, CancellationToken ct);
        Task<IReadOnlyList<TaskItem>> GetAllAsync(Guid userId, CancellationToken ct);
        Task UpdateAsync(TaskItem task, CancellationToken ct);
        Task DeleteAsync(TaskItem task, CancellationToken ct);
    }
}
