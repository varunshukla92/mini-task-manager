using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskService.Application.DTOs;
using TaskService.Application.Events;
using TaskService.Application.Exceptions;
using TaskService.Application.Interfaces;
using TaskService.Domain.Entities;

namespace TaskService.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;
        private readonly IEventPublisher _publisher;

        public TaskService(ITaskRepository repository, IEventPublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        private static TaskResponse MapToResponse(TaskItem task)
        {
            return new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
        public async Task<TaskResponse> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken ct)
        {
            var task = TaskItem.Create(
           userId,
           request.Title,
           request.Description,
           request.Priority,
           request.DueDate);

            await _repository.AddAsync(task,ct);

            await _publisher.PublishAsync(new TaskCreatedEvent
            {
                TaskId = task.Id,
                CreatedAt = task.CreatedAt,
                Description = task.Description,
                Priority = task.Priority,
                Title = task.Title
            });

            return MapToResponse(task);
        }

        public async Task<IReadOnlyList<TaskResponse>> GetAllAsync(Guid userId, CancellationToken ct)
        {
            var tasks = await _repository.GetAllAsync(userId, ct);
            return tasks.Select(MapToResponse).ToList();
        }

        public async Task<TaskResponse?> GetByIdAsync(Guid userId, Guid taskId, CancellationToken ct)
        {
            var task = await _repository.GetByIdAsync(taskId, userId,ct);

            if (task is null)
            {
                throw new NotFoundException($"Task with id '{taskId}' was not found.");
            }
            return task == null ? null : MapToResponse(task);
        }


        public async Task<bool> UpdateAsync(Guid userId, Guid taskId, UpdateTaskRequest request, CancellationToken ct)
        {
            var task = await _repository.GetByIdAsync(taskId, userId, ct);
            if (task is null)
            {
                throw new NotFoundException($"Task with id '{taskId}' was not found.");
            }

            task.Update(
                request.Title,
                request.Description,
                request.Status,
                request.Priority,
                request.DueDate);

            await _repository.UpdateAsync(task, ct);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid userId, Guid taskId, CancellationToken ct)
        {
            var task = await _repository.GetByIdAsync(taskId, userId, ct);
            if (task == null) return false;

            await _repository.DeleteAsync(task, ct);
            return true;
        }
    }
}
