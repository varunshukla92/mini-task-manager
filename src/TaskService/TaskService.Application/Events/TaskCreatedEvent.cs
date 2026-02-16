using TaskService.Domain.Enums;

namespace TaskService.Application.Events;

public class TaskCreatedEvent
{
    public Guid TaskId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; } = string.Empty;
    public TaskPriority Priority { get; set; }
    public DateTime CreatedAt { get; init; }
}
