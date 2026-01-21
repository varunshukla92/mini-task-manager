using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskService.Domain.Enums;

namespace TaskService.Application.DTOs
{
    public class CreateTaskRequest
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
