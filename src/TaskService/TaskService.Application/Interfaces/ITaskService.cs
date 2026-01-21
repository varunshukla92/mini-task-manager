using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskService.Application.DTOs;

namespace TaskService.Application.Interfaces
{
    public interface ITaskService
    {
        Task<TaskResponse> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken ct);
        Task<IReadOnlyList<TaskResponse>> GetAllAsync(Guid userId, CancellationToken ct);
        Task<TaskResponse?> GetByIdAsync(Guid userId, Guid taskId, CancellationToken ct);
        Task<bool> UpdateAsync(Guid userId, Guid taskId, UpdateTaskRequest request, CancellationToken ct);
        Task<bool> DeleteAsync(Guid userId, Guid taskId, CancellationToken ct);
    }
}
