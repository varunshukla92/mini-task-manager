using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskService.Application.Interfaces;
using TaskService.Application.DTOs;
using TaskService.API.Extensions;

namespace TaskService.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/tasks")]
[ApiVersion("1.0")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("Task creation started for userId {UserId}", userId);

        var task = await _taskService.CreateAsync(userId, request, cancellationToken);

        _logger.LogInformation("Task creation successful for userId {UserId}, taskId {TaskId}", userId, task.Id);

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id, version = "1.0" },
            task);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("Fetching task {TaskId} for userId {UserId}", id, userId);

        var task = await _taskService.GetByIdAsync(userId, id, cancellationToken);

        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} not found for userId {UserId}", id, userId);
            return NotFound();
        }

        _logger.LogInformation("Task {TaskId} retrieved successfully for userId {UserId}", id, userId);
        return Ok(task);
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllTask(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("Fetching all tasks for userId {UserId}", userId);

        IReadOnlyList<TaskResponse> taskList = await _taskService.GetAllAsync(userId, cancellationToken);

        _logger.LogInformation("Retrieved {Count} tasks for userId {UserId}", taskList?.Count ?? 0, userId);

        return Ok(taskList ?? Array.Empty<TaskResponse>());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("Updating task {TaskId} for userId {UserId}", id, userId);

        await _taskService.UpdateAsync(userId, id, request, cancellationToken);

        _logger.LogInformation("Task {TaskId} updated successfully for userId {UserId}", id, userId);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        _logger.LogInformation("Deleting task {TaskId} for userId {UserId}", id, userId);

        await _taskService.DeleteAsync(userId, id, cancellationToken);

        _logger.LogInformation("Task {TaskId} deleted successfully for userId {UserId}", id, userId);
        return NoContent();
    }
}
