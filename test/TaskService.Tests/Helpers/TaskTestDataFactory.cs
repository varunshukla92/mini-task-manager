using TaskService.Application.DTOs;
using TaskService.Domain.Entities;
using TaskService.Domain.Enums;
using TaskStatus = TaskService.Domain.Enums.TaskStatus;

namespace TaskService.Tests.Helpers;

public static class TaskTestDataFactory
{
    public static Guid CreateUserId()
        => Guid.NewGuid();

    public static TaskItem CreateTask(
        Guid userId,
        string title = "Test Task",
        string description = "Test Description",
        TaskPriority priority = TaskPriority.High,
        DateTime? dueDate = null)
    {
        return TaskItem.Create(
            userId,
            title,
            description,
            priority,
            dueDate ?? DateTime.UtcNow.AddDays(1)
        );
    }

    public static CreateTaskRequest CreateCreateTaskRequest(
        string title = "Test Task",
        string description = "Test Description",
        TaskPriority priority = TaskPriority.High,
        DateTime? dueDate = null)
    {
        return new CreateTaskRequest
        {
            Title = title,
            Description = description,
            Priority = priority,
            DueDate = dueDate ?? DateTime.UtcNow.AddDays(1)
        };
    }

    public static UpdateTaskRequest CreateUpdateTaskRequest(
        string title = "Updated Task",
        string description = "Updated Description",
        TaskStatus status = TaskStatus.InProgress,
        TaskPriority priority = TaskPriority.High,
        DateTime? dueDate = null)
    {
        return new UpdateTaskRequest
        {
            Title = title,
            Description = description,
            Status = status,
            Priority = priority,
            DueDate = dueDate ?? DateTime.UtcNow.AddDays(2)
        };
    }
}
