using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskService.Domain.Common;
using TaskService.Domain.Enums;
using TaskStatus = TaskService.Domain.Enums.TaskStatus;

namespace TaskService.Domain.Entities
{
    public class TaskItem : BaseEntity
    {
        public string Title { get; private set; } = default!;
        public string? Description { get; private set; }

        public TaskStatus Status { get; private set; }
        public TaskPriority Priority { get; private set; }

        public DateTime? DueDate { get; private set; }

        public Guid UserId { get; private set; }

        protected TaskItem() { }

        private TaskItem(
        Guid id,
        Guid userId,
        string title,
        string? description,
        TaskPriority priority,
        DateTime? dueDate)
        {
            Id = id;
            UserId = userId;
            Title = title;
            Description = description;
            Priority = priority;
            DueDate = dueDate;

            Status = TaskStatus.Todo;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public static TaskItem Create(
        Guid userId,
        string title,
        string? description,
        TaskPriority priority,
        DateTime? dueDate)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty");

            return new TaskItem(
                Guid.NewGuid(),
                userId,
                title.Trim(),
                description,
                priority,
                dueDate);
        }

        public void Update(
        string title,
        string? description,
        TaskStatus status,
        TaskPriority priority,
        DateTime? dueDate)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty");

            Title = title.Trim();
            Description = description;
            Status = status;
            Priority = priority;
            DueDate = dueDate;

            UpdatedAt = DateTime.UtcNow;
        }

    }
}
