namespace TaskProcessor.Functions.Models;

public class TaskCreatedEvent
{
    public Guid TaskId { get; init; }
    public string Title { get; init; }
    public string AssignedTo { get; init; }
    public DateTime DueDate { get; init; }
    public int Priority { get; init; }
    public DateTime CreatedAt { get; init; }
}
